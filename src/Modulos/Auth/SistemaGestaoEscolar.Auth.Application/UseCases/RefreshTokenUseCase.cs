using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Application.DTOs;
using SistemaGestaoEscolar.Auth.Application.Interfaces;
using SistemaGestaoEscolar.Auth.Domain.Repositories;

namespace SistemaGestaoEscolar.Auth.Application.UseCases;

/// <summary>
/// Caso de uso para renovação de token de acesso.
/// Valida refresh token e gera novos tokens.
/// </summary>
public class RefreshTokenUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RefreshTokenUseCase> _logger;

    public RefreshTokenUseCase(
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        ITokenService tokenService,
        ILogger<RefreshTokenUseCase> logger)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Executa o caso de uso de renovação de token
    /// </summary>
    public async Task<AuthResponseDto> ExecuteAsync(RefreshTokenDto refreshTokenDto)
    {
        try
        {
            _logger.LogInformation("Iniciando renovação de token");

            // 1. Validar entrada
            if (string.IsNullOrWhiteSpace(refreshTokenDto.RefreshToken))
            {
                _logger.LogWarning("Tentativa de renovação com refresh token vazio");
                throw new UnauthorizedAccessException("Refresh token é obrigatório");
            }

            // 2. Extrair informações do access token (se fornecido)
            Guid? userId = null;
            if (!string.IsNullOrWhiteSpace(refreshTokenDto.TokenAcesso))
            {
                userId = _tokenService.ExtractUserIdFromToken(refreshTokenDto.TokenAcesso);
            }

            // 3. Buscar usuário com refresh token válido
            var users = await _userRepository.ObterUsuariosComRefreshTokenValidoAsync();
            var user = users.FirstOrDefault(u => u.RefreshTokenEhValido(refreshTokenDto.RefreshToken));

            if (user == null)
            {
                _logger.LogWarning("Refresh token inválido ou expirado");
                throw new UnauthorizedAccessException("Refresh token inválido ou expirado");
            }

            // 4. Validar se o usuário do access token corresponde ao do refresh token
            if (userId.HasValue && userId.Value != user.Id)
            {
                _logger.LogWarning("Mismatch entre usuário do access token e refresh token");
                throw new UnauthorizedAccessException("Tokens não correspondem");
            }

            // 5. Verificar se o usuário ainda está ativo
            if (!user.Ativo)
            {
                _logger.LogWarning("Tentativa de renovação para usuário inativo: {UserId}", user.Id);
                throw new UnauthorizedAccessException("Usuário inativo");
            }

            // 6. Verificar se a conta não está bloqueada
            if (user.EstaBloqueada())
            {
                _logger.LogWarning("Tentativa de renovação para usuário bloqueado: {UserId}", user.Id);
                throw new UnauthorizedAccessException("Conta bloqueada");
            }

            // 7. Gerar novos tokens
            var newAccessToken = await _tokenService.GenerateAccessTokenAsync(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // 8. Atualizar refresh token no usuário
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7); // 7 dias padrão
            user.DefinirRefreshToken(newRefreshToken, refreshTokenExpiry);

            // 9. Atualizar atividade da sessão (se possível identificar)
            var activeSessions = await _sessionRepository.ObterSessoesAtivasPorUsuarioAsync(user.Id);
            var currentSession = activeSessions.FirstOrDefault(s => 
                s.EhMesmoDispositivo(refreshTokenDto.EnderecoIp ?? "unknown", refreshTokenDto.AgenteUsuario ?? "unknown"));
            
            if (currentSession != null)
            {
                currentSession.AtualizarAtividade();
                _sessionRepository.Atualizar(currentSession);
            }

            // 10. Salvar alterações
            _userRepository.Atualizar(user);
            await _userRepository.SalvarAlteracoesAsync();
            
            if (currentSession != null)
            {
                await _sessionRepository.SalvarAlteracoesAsync();
            }

            _logger.LogInformation("Token renovado com sucesso para usuário: {UserId}", user.Id);

            // 11. Retornar resposta
            return new AuthResponseDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                TipoToken = "Bearer",
                ExpiraEm = _tokenService.GetTokenExpirationInSeconds(),
                ExpiraEm_DateTime = _tokenService.GetTokenExpirationDateTime(),
                Usuario = new UserInfoDto
                {
                    Id = user.Id,
                    NomeCompleto = user.NomeCompleto,
                    Email = user.Email.Value,
                    CodigoPerfil = user.Perfil.Code,
                    NomePerfil = user.Perfil.Name,
                    Iniciais = user.Iniciais,
                    EmailConfirmado = user.EmailConfirmado,
                    UltimoLoginEm = user.UltimoLoginEm,
                    EscolaId = user.EscolaId
                },
                RequerMudancaSenha = user.SenhaPrecisaSerAlterada(),
                RequerConfirmacaoEmail = !user.EmailConfirmado,
                Permissoes = await GetUserPermissionsAsync(user),
                Escolas = await GetUserSchoolsAsync(user)
            };
        }
        catch (UnauthorizedAccessException)
        {
            throw; // Re-throw authorization exceptions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado durante renovação de token");
            throw new InvalidOperationException("Erro interno durante renovação de token");
        }
    }

    /// <summary>
    /// Obtém as permissões do usuário baseadas no seu papel
    /// </summary>
    private Task<IEnumerable<string>> GetUserPermissionsAsync(Domain.Entities.User user)
    {
        var permissions = new List<string>();

        // Permissões baseadas no papel
        if (user.Perfil.Level >= 3) // Níveis mais altos podem acessar dados acadêmicos
            permissions.Add("academic.read");

        if (user.Perfil.Level >= 4) // Níveis mais altos podem acessar dados financeiros
            permissions.Add("financial.read");

        if (user.Perfil.Level >= 3) // Níveis mais altos podem gerar relatórios
            permissions.Add("reports.generate");

        if (user.Perfil.Level >= 5) // Apenas níveis mais altos podem configurar sistema
            permissions.Add("system.configure");

        // Permissões específicas por papel
        switch (user.Perfil.Code)
        {
            case "SuperAdmin":
                permissions.AddRange(new[] { "users.manage", "schools.manage", "system.admin" });
                break;
            case "Admin":
                permissions.AddRange(new[] { "users.manage", "schools.read" });
                break;
            case "Director":
                permissions.AddRange(new[] { "users.read", "academic.manage", "reports.view" });
                break;
            case "Teacher":
                permissions.AddRange(new[] { "students.read", "grades.manage", "attendance.manage" });
                break;
            case "Parent":
                permissions.AddRange(new[] { "children.read", "grades.read", "attendance.read" });
                break;
        }

        return Task.FromResult(permissions.Distinct());
    }

    /// <summary>
    /// Obtém as escolas que o usuário pode acessar
    /// </summary>
    private async Task<IEnumerable<SchoolAccessDto>> GetUserSchoolsAsync(Domain.Entities.User user)
    {
        var schools = new List<SchoolAccessDto>();

        // SuperAdmin pode acessar todas as escolas
        if (user.Perfil == Domain.ValueObjects.UserRole.SuperAdmin)
        {
            // TODO: Buscar todas as escolas do sistema
            // Por enquanto, retorna vazio - será implementado quando integrar com módulo de escolas
        }
        else if (user.EscolaId.HasValue)
        {
            // Usuário tem acesso apenas à sua escola
            // TODO: Buscar dados da escola específica
            schools.Add(new SchoolAccessDto
            {
                Id = user.EscolaId.Value,
                Nome = "Escola do Usuário", // Placeholder
                EhPrincipal = true,
                Permissoes = await GetUserPermissionsAsync(user)
            });
        }

        return schools;
    }
}