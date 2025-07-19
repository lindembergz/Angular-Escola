using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Domain.Entities;
using SistemaGestaoEscolar.Auth.Domain.Repositories;
using SistemaGestaoEscolar.Auth.Domain.Services;
using SistemaGestaoEscolar.Auth.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Auth.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de domínio de autenticação.
/// Contém lógica de negócio complexa que não pertence a uma entidade específica.
/// </summary>
public class AuthDomainService : IAuthDomainService
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ILogger<AuthDomainService> _logger;

    // Cache simples para IPs suspeitos (em produção, usar Redis)
    private static readonly HashSet<string> SuspiciousIps = new();
    private static readonly Dictionary<string, List<DateTime>> LoginAttempts = new();

    public AuthDomainService(
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        IPasswordHashService passwordHashService,
        ILogger<AuthDomainService> logger)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _passwordHashService = passwordHashService;
        _logger = logger;
    }

    /// <summary>
    /// Valida se um usuário pode fazer login
    /// </summary>
    public async Task<LoginValidationResult> ValidateLoginAsync(User user, string password, string ipAddress, string userAgent)
    {
        try
        {
            // 1. Verificar se o usuário está ativo
            if (!user.Ativo)
            {
                _logger.LogWarning("Tentativa de login para usuário inativo: {UserId}", user.Id);
                return new LoginValidationResult(false, "Conta inativa");
            }

            // 2. Verificar se a conta não está bloqueada
            if (user.EstaBloqueada())
            {
                _logger.LogWarning("Tentativa de login para usuário bloqueado: {UserId}", user.Id);
                var lockoutDuration = user.BloqueadoAte!.Value - DateTime.UtcNow;
                return new LoginValidationResult(false, "Conta temporariamente bloqueada", LockoutDuration: lockoutDuration);
            }

            // 3. Verificar se há muitas tentativas do IP
            if (await HasTooManyLoginAttemptsFromIpAsync(ipAddress, TimeSpan.FromMinutes(15)))
            {
                _logger.LogWarning("Muitas tentativas de login do IP: {IpAddress}", ipAddress);
                return new LoginValidationResult(false, "Muitas tentativas de login. Tente novamente em alguns minutos.");
            }

            // 4. Verificar se o IP é suspeito
            var isSuspiciousIp = await IsSuspiciousIpAsync(ipAddress);
            if (isSuspiciousIp)
            {
                _logger.LogWarning("Tentativa de login de IP suspeito: {IpAddress}", ipAddress);
            }

            // 5. Verificar a senha
            if (!user.VerificarSenha(password))
            {
                _logger.LogWarning("Senha incorreta para usuário: {UserId}", user.Id);
                await RegisterLoginAttemptAsync(ipAddress);
                return new LoginValidationResult(false, "Credenciais inválidas");
            }

            // 6. Verificar se precisa alterar senha
            var requiresPasswordChange = ShouldForcePasswordChange(user);

            // 7. Verificar se precisa confirmar email
            var requiresEmailConfirmation = !user.EmailConfirmado;

            // Login válido
            await RegisterLoginAttemptAsync(ipAddress, success: true);
            
            return new LoginValidationResult(
                true,
                RequiresPasswordChange: requiresPasswordChange,
                RequiresEmailConfirmation: requiresEmailConfirmation,
                IsSuspicious: isSuspiciousIp
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante validação de login para usuário: {UserId}", user.Id);
            return new LoginValidationResult(false, "Erro interno durante autenticação");
        }
    }

    /// <summary>
    /// Valida se um email pode ser usado para criar uma nova conta
    /// </summary>
    public async Task<bool> CanCreateAccountWithEmailAsync(Email email)
    {
        try
        {
            // Verificar se o email já existe
            var emailExists = await _userRepository.ExistePorEmailAsync(email);
            if (emailExists)
            {
                return false;
            }

            // Verificar domínios bloqueados (exemplo)
            var blockedDomains = new[] { "tempmail.com", "10minutemail.com", "guerrillamail.com" };
            var domain = email.GetDomain();
            
            if (blockedDomains.Contains(domain.ToLowerInvariant()))
            {
                _logger.LogWarning("Tentativa de criar conta com domínio bloqueado: {Domain}", domain);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar email para criação de conta: {Email}", email.Value);
            return false;
        }
    }

    /// <summary>
    /// Valida se um usuário pode alterar seu papel
    /// </summary>
    public bool CanChangeRole(User currentUser, User targetUser, UserRole newRole)
    {
        // SuperAdmin pode alterar qualquer papel
        if (currentUser.Perfil == UserRole.SuperAdmin)
            return true;

        // Admin pode alterar papéis exceto SuperAdmin
        if (currentUser.Perfil == UserRole.Admin && newRole != UserRole.SuperAdmin)
            return true;

        // Diretor pode alterar papéis de nível inferior
        if (currentUser.Perfil == UserRole.Director && 
            newRole.Level <= UserRole.Teacher.Level && 
            newRole != UserRole.SuperAdmin && 
            newRole != UserRole.Admin)
            return true;

        return false;
    }

    /// <summary>
    /// Valida se um usuário pode gerenciar outro usuário
    /// </summary>
    public bool PodeGerenciarUsuario(User manager, User? targetUser)
    {
        return targetUser != null && manager.Perfil.PodeGerenciar(targetUser.Perfil);
    }

    /// <summary>
    /// Valida se um usuário pode acessar uma escola
    /// </summary>
    public bool CanAccessSchool(User user, Guid schoolId)
    {
        return user.PodeAcessarEscola(schoolId);
    }

    /// <summary>
    /// Gera um token de recuperação de senha
    /// </summary>
    public string GeneratePasswordResetToken(User user)
    {
        // Implementação delegada para TokenService
        throw new NotImplementedException("Use ITokenService.GeneratePasswordResetToken");
    }

    /// <summary>
    /// Valida um token de recuperação de senha
    /// </summary>
    public bool ValidatePasswordResetToken(User user, string token)
    {
        // Implementação delegada para TokenService
        throw new NotImplementedException("Use ITokenService.ValidatePasswordResetToken");
    }

    /// <summary>
    /// Gera um token de confirmação de email
    /// </summary>
    public string GenerateEmailConfirmationToken(User user)
    {
        // Implementação delegada para TokenService
        throw new NotImplementedException("Use ITokenService.GenerateEmailConfirmationToken");
    }

    /// <summary>
    /// Valida um token de confirmação de email
    /// </summary>
    public bool ValidateEmailConfirmationToken(User user, string token)
    {
        // Implementação delegada para TokenService
        throw new NotImplementedException("Use ITokenService.ValidateEmailConfirmationToken");
    }

    /// <summary>
    /// Verifica se um IP está na lista de IPs suspeitos
    /// </summary>
    public async Task<bool> IsSuspiciousIpAsync(string ipAddress)
    {
        try
        {
            // Verificar cache local
            if (SuspiciousIps.Contains(ipAddress))
                return true;

            // Verificar padrões suspeitos
            if (await HasSuspiciousPatternAsync(ipAddress))
            {
                SuspiciousIps.Add(ipAddress);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar IP suspeito: {IpAddress}", ipAddress);
            return false;
        }
    }

    /// <summary>
    /// Verifica se há muitas tentativas de login de um IP
    /// </summary>
    public Task<bool> HasTooManyLoginAttemptsFromIpAsync(string ipAddress, TimeSpan timeWindow)
    {
        try
        {
            if (!LoginAttempts.ContainsKey(ipAddress))
                return Task.FromResult(false);

            var attempts = LoginAttempts[ipAddress];
            var cutoffTime = DateTime.UtcNow - timeWindow;

            // Remover tentativas antigas
            attempts.RemoveAll(attempt => attempt < cutoffTime);

            // Verificar se há muitas tentativas
            const int maxAttempts = 10;
            return Task.FromResult(attempts.Count >= maxAttempts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar tentativas de login do IP: {IpAddress}", ipAddress);
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Registra uma tentativa de login suspeita
    /// </summary>
    public async Task RegisterSuspiciousLoginAttemptAsync(string ipAddress, string userAgent, string email)
    {
        try
        {
            _logger.LogWarning("Tentativa de login suspeita - IP: {IpAddress}, UserAgent: {UserAgent}, Email: {Email}", 
                ipAddress, userAgent, email);

            // Adicionar IP à lista de suspeitos se houver muitas tentativas
            await RegisterLoginAttemptAsync(ipAddress, success: false);
            
            if (await HasTooManyLoginAttemptsFromIpAsync(ipAddress, TimeSpan.FromHours(1)))
            {
                SuspiciousIps.Add(ipAddress);
                _logger.LogWarning("IP adicionado à lista de suspeitos: {IpAddress}", ipAddress);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar tentativa suspeita");
        }
    }

    /// <summary>
    /// Limpa dados de segurança antigos
    /// </summary>
    public void CleanupSecurityData()
    {
        try
        {
            // Limpar tentativas de login antigas
            var cutoffTime = DateTime.UtcNow.AddHours(-24);
            var keysToRemove = new List<string>();
            
            foreach (var kvp in LoginAttempts)
            {
                kvp.Value.RemoveAll(attempt => attempt < cutoffTime);
                if (!kvp.Value.Any())
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
            
            foreach (var key in keysToRemove)
            {
                LoginAttempts.Remove(key);
            }

            // Limpar sessões expiradas
            _sessionRepository.RemoverSessoesExpiradasAsync();
            
            // Limpar refresh tokens expirados
            _userRepository.LimparRefreshTokensExpiradosAsync();
            
            _logger.LogInformation("Limpeza de dados de segurança concluída");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante limpeza de dados de segurança");
        }
    }

    /// <summary>
    /// Obtém recomendações de segurança para um usuário
    /// </summary>
    public async Task<IEnumerable<SecurityRecommendation>> GetSecurityRecommendationsAsync(User user)
    {
        var recommendations = new List<SecurityRecommendation>();

        try
        {
            // Verificar se precisa alterar senha
            if (ShouldForcePasswordChange(user))
            {
                recommendations.Add(new SecurityRecommendation(
                    "Alterar Senha",
                    "Sua senha não é alterada há muito tempo. Considere criar uma nova senha.",
                    SecurityRecommendationType.PasswordChange,
                    SecurityRecommendationPriority.High
                ));
            }

            // Verificar se email não está confirmado
            if (!user.EmailConfirmado)
            {
                recommendations.Add(new SecurityRecommendation(
                    "Confirmar Email",
                    "Confirme seu endereço de email para aumentar a segurança da conta.",
                    SecurityRecommendationType.UpdateContactInfo,
                    SecurityRecommendationPriority.Medium
                ));
            }

            // Verificar sessões ativas
            var activeSessions = await _sessionRepository.ObterSessoesAtivasPorUsuarioAsync(user.Id);
            if (activeSessions.Count() > 3)
            {
                recommendations.Add(new SecurityRecommendation(
                    "Revisar Sessões",
                    "Você tem muitas sessões ativas. Revise e encerre sessões desnecessárias.",
                    SecurityRecommendationType.ReviewSessions,
                    SecurityRecommendationPriority.Medium
                ));
            }

            // Verificar sessões suspeitas
            var suspiciousSessions = activeSessions.Where(s => s.EhSuspeita());
            if (suspiciousSessions.Any())
            {
                recommendations.Add(new SecurityRecommendation(
                    "Sessões Suspeitas",
                    "Detectamos atividade suspeita em sua conta. Revise suas sessões ativas.",
                    SecurityRecommendationType.SecurityAudit,
                    SecurityRecommendationPriority.Critical
                ));
            }

            return recommendations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter recomendações de segurança para usuário: {UserId}", user.Id);
            return recommendations;
        }
    }

    /// <summary>
    /// Calcula o nível de risco de uma sessão
    /// </summary>
    public async Task<RiskLevel> CalculateSessionRiskAsync(Session session)
    {
        try
        {
            var riskScore = 0;

            // Verificar se é suspeita
            if (session.EhSuspeita())
                riskScore += 40;

            // Verificar IP suspeito
            if (await IsSuspiciousIpAsync(session.EnderecoIp))
                riskScore += 30;

            // Verificar duração
            if (session.EhLongaDuracao())
                riskScore += 20;

            // Verificar inatividade
            if (session.Expirou())
                riskScore += 25;

            // Verificar dispositivo móvel (menor risco)
            if (session.EhDispositivoMovel())
                riskScore -= 10;

            return riskScore switch
            {
                >= 70 => RiskLevel.Critical,
                >= 50 => RiskLevel.High,
                >= 30 => RiskLevel.Medium,
                _ => RiskLevel.Low
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao calcular risco da sessão: {SessionId}", session.Id);
            return RiskLevel.Medium;
        }
    }

    /// <summary>
    /// Verifica se um usuário deve ser forçado a alterar a senha
    /// </summary>
    public bool ShouldForcePasswordChange(User user)
    {
        // Política: alterar senha a cada 90 dias
        return user.SenhaPrecisaSerAlterada(90);
    }

    /// <summary>
    /// Verifica se um usuário deve ser desativado por inatividade
    /// </summary>
    public bool ShouldDeactivateForInactivity(User user)
    {
        // Política: desativar após 180 dias sem login
        return user.EstaInativo(180);
    }

    #region Private Methods

    private async Task<bool> HasSuspiciousPatternAsync(string ipAddress)
    {
        try
        {
            // Verificar se há muitas sessões do mesmo IP
            var recentSessions = await _sessionRepository.ObterSessoesPorEnderecoIpAsync(ipAddress);
            var recentCount = recentSessions.Count(s => s.IniciadaEm > DateTime.UtcNow.AddHours(-1));
            
            if (recentCount > 20) // Mais de 20 sessões na última hora
                return true;

            // Verificar padrões de User Agent suspeitos
            var suspiciousUserAgents = recentSessions
                .Where(s => s.AgenteUsuario.ToLowerInvariant().Contains("bot") ||
                           s.AgenteUsuario.ToLowerInvariant().Contains("crawler") ||
                           s.AgenteUsuario.ToLowerInvariant().Contains("spider"))
                .Count();
                
            if (suspiciousUserAgents > 5)
                return true;

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar padrões suspeitos para IP: {IpAddress}", ipAddress);
            return false;
        }
    }

    private Task RegisterLoginAttemptAsync(string ipAddress, bool success = false)
    {
        try
        {
            if (!LoginAttempts.ContainsKey(ipAddress))
            {
                LoginAttempts[ipAddress] = new List<DateTime>();
            }

            LoginAttempts[ipAddress].Add(DateTime.UtcNow);

            // Limitar histórico a 50 tentativas por IP
            if (LoginAttempts[ipAddress].Count > 50)
            {
                LoginAttempts[ipAddress] = LoginAttempts[ipAddress]
                    .OrderByDescending(x => x)
                    .Take(50)
                    .ToList();
            }
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar tentativa de login para IP: {IpAddress}", ipAddress);
            return Task.CompletedTask;
        }
    }

    #endregion
}