using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Application.DTOs;
using SistemaGestaoEscolar.Auth.Application.Interfaces;
using SistemaGestaoEscolar.Auth.Domain.Entities;
using SistemaGestaoEscolar.Auth.Domain.Repositories;
using SistemaGestaoEscolar.Auth.Domain.Services;
using SistemaGestaoEscolar.Auth.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Auth.Application.UseCases;

/// <summary>
/// Caso de uso para login de usuário.
/// Implementa toda a lógica de autenticação seguindo princípios de Clean Architecture.
/// </summary>
public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IAuthDomainService _authDomainService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginUseCase> _logger;

    public LoginUseCase(
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        IAuthDomainService authDomainService,
        ITokenService tokenService,
        ILogger<LoginUseCase> logger)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _authDomainService = authDomainService;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Executa o caso de uso de login
    /// </summary>
    public async Task<AuthResponseDto> ExecuteAsync(LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation("Iniciando processo de login para email: {Email}", loginDto.Email);

            // 1. Validar entrada
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Senha))
            {
                _logger.LogWarning("Tentativa de login com credenciais vazias");
                throw new UnauthorizedAccessException("Email e senha são obrigatórios");
            }

            // 2. Buscar usuário por email
            var user = await _userRepository.ObterPorEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Tentativa de login com email não encontrado: {Email}", loginDto.Email);
                
                // Registrar tentativa suspeita
                await _authDomainService.RegisterSuspiciousLoginAttemptAsync(
                    loginDto.EnderecoIp ?? "unknown", 
                    loginDto.AgenteUsuario ?? "unknown", 
                    loginDto.Email);
                
                throw new UnauthorizedAccessException("Credenciais inválidas");
            }

            // 3. Validar login usando serviço de domínio
            var validationResult = await _authDomainService.ValidateLoginAsync(
                user, 
                loginDto.Senha, 
                loginDto.EnderecoIp ?? "unknown", 
                loginDto.AgenteUsuario ?? "unknown");

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Falha na validação de login para usuário {UserId}: {Error}", 
                    user.Id, validationResult.ErrorMessage);

                // Registrar tentativa falhada
                user.RegistrarLoginFalhado();
                _userRepository.Atualizar(user);
                await _userRepository.SalvarAlteracoesAsync();

                throw new UnauthorizedAccessException(validationResult.ErrorMessage ?? "Credenciais inválidas");
            }

            // 4. Registrar login bem-sucedido
            user.RegistrarLoginBemSucedido();

            // 5. Criar nova sessão
            var session = user.CriarSessao(
                loginDto.EnderecoIp ?? "unknown", 
                loginDto.AgenteUsuario ?? "unknown");

            await _sessionRepository.AdicionarAsync(session);

            // 6. Gerar tokens
            var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // 7. Configurar refresh token
            var refreshTokenExpiry = loginDto.LembrarMe 
                ? DateTime.UtcNow.AddDays(30)  // 30 dias se "lembrar de mim"
                : DateTime.UtcNow.AddDays(7);  // 7 dias padrão

            user.DefinirRefreshToken(refreshToken, refreshTokenExpiry);

            // 8. Salvar alterações
            _userRepository.Atualizar(user);
            await _userRepository.SalvarAlteracoesAsync();
            await _sessionRepository.SalvarAlteracoesAsync();

            _logger.LogInformation("Login realizado com sucesso para usuário {UserId}", user.Id);

            // 9. Retornar resposta
            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                TipoToken = "Bearer",
                ExpiraEm = _tokenService.GetTokenExpirationInSeconds(),
                ExpiraEm_DateTime = _tokenService.GetTokenExpirationDateTime(),
                Usuario = new UserInfoDto
                {
                    Id = user.Id,
                    PrimeiroNome = user.PrimeiroNome,
                    UltimoNome = user.UltimoNome,
                    NomeCompleto = user.NomeCompleto,
                    Email = user.Email.Value,
                    CodigoPerfil = user.Perfil.Code,
                    NomePerfil = user.Perfil.Name,
                    NivelPerfil = user.Perfil.Level,
                    Iniciais = user.Iniciais,
                    Ativo = user.Ativo,
                    EmailConfirmado = user.EmailConfirmado,
                    UltimoLoginEm = user.UltimoLoginEm,
                    EscolaId = user.EscolaId,
                    Permissoes = await GetUserPermissionsAsync(user)
                },
                RequerMudancaSenha = validationResult.RequiresPasswordChange,
                RequerConfirmacaoEmail = validationResult.RequiresEmailConfirmation,
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
            _logger.LogError(ex, "Erro inesperado durante login para email: {Email}", loginDto.Email);
            throw new InvalidOperationException("Erro interno durante autenticação");
        }
    }

    /// <summary>
    /// Obtém as permissões do usuário baseadas no seu papel
    /// </summary>
    private Task<IEnumerable<string>> GetUserPermissionsAsync(User user)
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
    private async Task<IEnumerable<SchoolAccessDto>> GetUserSchoolsAsync(User user)
    {
        var schools = new List<SchoolAccessDto>();

        // SuperAdmin pode acessar todas as escolas
        if (user.Perfil == UserRole.SuperAdmin)
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