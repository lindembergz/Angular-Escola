using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SistemaGestaoEscolar.Auth.Application.Interfaces;
using SistemaGestaoEscolar.Auth.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SistemaGestaoEscolar.Auth.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de tokens JWT.
/// Gera, valida e gerencia tokens de acesso e refresh tokens.
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _tokenHandler = new JwtSecurityTokenHandler();
        _tokenValidationParameters = CreateTokenValidationParameters();
    }

    /// <summary>
    /// Gera um token de acesso JWT para o usuário
    /// </summary>
    public async Task<string> GenerateAccessTokenAsync(User user)
    {
        try
        {
            var claims = await GetUserClaimsAsync(user);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtSecret()));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = GetTokenExpirationDateTime(),
                SigningCredentials = credentials,
                Issuer = GetJwtIssuer(),
                Audience = GetJwtAudience(),
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow
            };

            var token = _tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = _tokenHandler.WriteToken(token);

            _logger.LogDebug("Token de acesso gerado para usuário: {UserId}", user.Id);
            
            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar token de acesso para usuário: {UserId}", user.Id);
            throw new InvalidOperationException("Erro ao gerar token de acesso", ex);
        }
    }

    /// <summary>
    /// Gera um refresh token para o usuário
    /// </summary>
        public string GenerateRefreshToken()
    {
        try
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            
            var refreshToken = Convert.ToBase64String(randomBytes);
            
            _logger.LogDebug("Refresh token gerado");
            
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar refresh token");
            throw new InvalidOperationException("Erro ao gerar refresh token", ex);
        }
    }

    /// <summary>
    /// Valida um token JWT
    /// </summary>
    public Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
                return Task.FromResult<ClaimsPrincipal?>(null);

            var principal = _tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);

            // Verificar se é um JWT válido
            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning("Token com algoritmo inválido");
                return Task.FromResult<ClaimsPrincipal?>(null);
            }

            _logger.LogDebug("Token validado com sucesso");

            // Retornar principal como possivelmente nulo, mas aqui ele é válido
            return Task.FromResult<ClaimsPrincipal?>(principal);
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogDebug(ex, "Token expirado");
            return Task.FromResult<ClaimsPrincipal?>(null);
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "Token inválido");
            return Task.FromResult<ClaimsPrincipal?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao validar token");
            return Task.FromResult<ClaimsPrincipal?>(null);
        }
    }

    /// <summary>
    /// Obtém claims do usuário para o token
    /// </summary>
    public  Task<IEnumerable<Claim>> GetUserClaimsAsync(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.NomeCompleto),
            new(ClaimTypes.Email, user.Email.Value),
            new(ClaimTypes.Role, user.Perfil.Code),
            new("role_name", user.Perfil.Name),
            new("role_level", user.Perfil.Level.ToString()),
            new("email_confirmed", user.EmailConfirmado.ToString().ToLowerInvariant()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // Adicionar escola se o usuário tiver uma associada
        if (user.EscolaId.HasValue)
        {
            claims.Add(new Claim("school_id", user.EscolaId.Value.ToString()));
        }

        // Adicionar permissões baseadas no papel
        var permissions = GetPermissionsForRole(user.Perfil);
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        return Task.FromResult<IEnumerable<Claim>>(claims);
    }

    /// <summary>
    /// Obtém o tempo de expiração do token em segundos
    /// </summary>
    public int GetTokenExpirationInSeconds()
    {
        return _configuration.GetValue<int>("Jwt:ExpirationInMinutes", 60) * 60;
    }

    /// <summary>
    /// Obtém a data/hora de expiração do token
    /// </summary>
    public DateTime GetTokenExpirationDateTime()
    {
        var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationInMinutes", 60);
        return DateTime.UtcNow.AddMinutes(expirationMinutes);
    }

    /// <summary>
    /// Extrai o ID do usuário de um token
    /// </summary>
    public Guid? ExtractUserIdFromToken(string token)
    {
        try
        {
            var principal = _tokenHandler.ValidateToken(token, _tokenValidationParameters, out _);
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Extrai o email do usuário de um token
    /// </summary>
    public string? ExtractEmailFromToken(string token)
    {
        try
        {
            var principal = _tokenHandler.ValidateToken(token, _tokenValidationParameters, out _);
            return principal.FindFirst(ClaimTypes.Email)?.Value;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Verifica se um token está expirado
    /// </summary>
    public bool IsTokenExpired(string token)
    {
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo < DateTime.UtcNow;
        }
        catch
        {
            return true; // Se não conseguir ler, considera expirado
        }
    }

    /// <summary>
    /// Gera um token de recuperação de senha
    /// </summary>
    public string GeneratePasswordResetToken(User user)
    {
        try
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email.Value),
                new Claim("token_type", "password_reset"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetPasswordResetSecret()));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), // Token válido por 1 hora
                SigningCredentials = credentials,
                Issuer = GetJwtIssuer(),
                Audience = GetJwtAudience()
            };

            var token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar token de recuperação de senha");
            throw new InvalidOperationException("Erro ao gerar token de recuperação", ex);
        }
    }

    /// <summary>
    /// Valida um token de recuperação de senha
    /// </summary>
    public bool ValidatePasswordResetToken(User user, string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetPasswordResetSecret()));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = GetJwtIssuer(),
                ValidateAudience = true,
                ValidAudience = GetJwtAudience(),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = _tokenHandler.ValidateToken(token, validationParameters, out _);
            
            // Verificar se o token é para o usuário correto
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emailClaim = principal.FindFirst(ClaimTypes.Email)?.Value;
            var tokenTypeClaim = principal.FindFirst("token_type")?.Value;

            return Guid.TryParse(userIdClaim, out var tokenUserId) &&
                   tokenUserId == user.Id &&
                   emailClaim == user.Email.Value &&
                   tokenTypeClaim == "password_reset";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token de recuperação inválido para usuário: {UserId}", user.Id);
            return false;
        }
    }

    /// <summary>
    /// Gera um token de confirmação de email
    /// </summary>
    public string GenerateEmailConfirmationToken(User user)
    {
        try
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email.Value),
                new Claim("token_type", "email_confirmation"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetEmailConfirmationSecret()));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // Token válido por 7 dias
                SigningCredentials = credentials,
                Issuer = GetJwtIssuer(),
                Audience = GetJwtAudience()
            };

            var token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar token de confirmação de email");
            throw new InvalidOperationException("Erro ao gerar token de confirmação", ex);
        }
    }

    /// <summary>
    /// Valida um token de confirmação de email
    /// </summary>
    public bool ValidateEmailConfirmationToken(User user, string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetEmailConfirmationSecret()));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = GetJwtIssuer(),
                ValidateAudience = true,
                ValidAudience = GetJwtAudience(),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = _tokenHandler.ValidateToken(token, validationParameters, out _);
            
            // Verificar se o token é para o usuário correto
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emailClaim = principal.FindFirst(ClaimTypes.Email)?.Value;
            var tokenTypeClaim = principal.FindFirst("token_type")?.Value;

            return Guid.TryParse(userIdClaim, out var tokenUserId) &&
                   tokenUserId == user.Id &&
                   emailClaim == user.Email.Value &&
                   tokenTypeClaim == "email_confirmation";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token de confirmação inválido para usuário: {UserId}", user.Id);
            return false;
        }
    }

    #region Private Methods

    private TokenValidationParameters CreateTokenValidationParameters()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtSecret()));
        
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = GetJwtIssuer(),
            ValidateAudience = true,
            ValidAudience = GetJwtAudience(),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    private string GetJwtSecret()
    {
        var secret = _configuration["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("JWT Secret não configurado");
        }
        return secret;
    }

    private string GetPasswordResetSecret()
    {
        var secret = _configuration["Jwt:PasswordResetSecret"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            // Fallback para o secret principal com sufixo
            return GetJwtSecret() + "_password_reset";
        }
        return secret;
    }

    private string GetEmailConfirmationSecret()
    {
        var secret = _configuration["Jwt:EmailConfirmationSecret"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            // Fallback para o secret principal com sufixo
            return GetJwtSecret() + "_email_confirmation";
        }
        return secret;
    }

    private string GetJwtIssuer()
    {
        return _configuration["Jwt:Issuer"] ?? "SistemaGestaoEscolar";
    }

    private string GetJwtAudience()
    {
        return _configuration["Jwt:Audience"] ?? "SistemaGestaoEscolar.Users";
    }

    private static IEnumerable<string> GetPermissionsForRole(Domain.ValueObjects.UserRole role)
    {
        var permissions = new List<string>();

        // Permissões baseadas no papel
        if (role.PodeAcessarDadosAcademicos())
            permissions.Add("academic.read");

        if (role.PodeAcessarDadosFinanceiros())
            permissions.Add("financial.read");

        if (role.PodeGerarRelatorios())
            permissions.Add("reports.generate");

        if (role.PodeConfigurarSistema())
            permissions.Add("system.configure");

        // Permissões específicas por papel
        switch (role.Code)
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

        return permissions.Distinct();
    }

    #endregion
}