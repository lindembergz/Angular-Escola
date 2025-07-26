using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SistemaGestaoEscolar.Shared.Infrastructure.Authorization;
using SistemaGestaoEscolar.Shared.Infrastructure.Middleware;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SistemaGestaoEscolar.Shared.Infrastructure.Configuration;

/// <summary>
/// Extensão de configuração centralizada para todos os aspectos de segurança da API.
/// Consolida configuração de autenticação JWT, políticas de autorização e middleware de segurança.
/// </summary>
public static class SecurityConfiguration
{
    /// <summary>
    /// Adiciona e configura todos os serviços de segurança da API de forma centralizada.
    /// Inclui autenticação JWT, políticas de autorização e middleware de segurança.
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <param name="configuration">Configuração da aplicação</param>
    /// <returns>Coleção de serviços configurada</returns>
    public static IServiceCollection AddApiSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar autenticação JWT
        services.AddJwtAuthentication(configuration);

        // Configurar políticas de autorização centralizadas
        services.AddSystemAuthorization();

        // Configurar middleware de segurança
        services.AddSecurityMiddleware();

        // Configurar validação de configuração JWT
        services.AddJwtConfigurationValidation(configuration);

        return services;
    }

    /// <summary>
    /// Configura a autenticação JWT com validação robusta de tokens
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <param name="configuration">Configuração da aplicação</param>
    /// <returns>Coleção de serviços configurada</returns>
    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException(
            "JWT Secret não configurada. Verifique se a propriedade 'Jwt.Secret' está definida no appsettings.json com pelo menos 32 caracteres.");
        var issuer = jwtSettings["Issuer"] ?? "SistemaGestaoEscolar";
        var audience = jwtSettings["Audience"] ?? "SistemaGestaoEscolar.API";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // Para desenvolvimento - deve ser true em produção
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.FromMinutes(5), // Tolerância para diferenças de relógio
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };

            // Configurar eventos para logging de segurança
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("SecurityConfiguration.JWT");
                    logger.LogWarning("Falha na autenticação JWT: {Error} - IP: {IP} - User-Agent: {UserAgent}",
                        context.Exception.Message,
                        context.HttpContext.Connection.RemoteIpAddress,
                        context.HttpContext.Request.Headers["User-Agent"].ToString());
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("SecurityConfiguration.JWT");
                    logger.LogWarning("Desafio de autenticação JWT - IP: {IP} - Path: {Path}",
                        context.HttpContext.Connection.RemoteIpAddress,
                        context.HttpContext.Request.Path);
                    return Task.CompletedTask;
                },
                OnForbidden = context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("SecurityConfiguration.JWT");
                    logger.LogWarning("Acesso proibido - User: {User} - IP: {IP} - Path: {Path}",
                        context.HttpContext.User.Identity?.Name ?? "Anonymous",
                        context.HttpContext.Connection.RemoteIpAddress,
                        context.HttpContext.Request.Path);
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Configura middleware de segurança personalizado
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <returns>Coleção de serviços configurada</returns>
    private static IServiceCollection AddSecurityMiddleware(this IServiceCollection services)
    {
        // Middlewares não devem ser registrados como serviços - eles são usados apenas no pipeline
        // Os middlewares SecurityLoggingMiddleware e AuthorizationErrorMiddleware são adicionados
        // diretamente no pipeline através do método UseMiddleware<T>() no Program.cs

        // Configurar session para tracking de segurança
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
        });

        return services;
    }

    /// <summary>
    /// Adiciona validação de configuração JWT na inicialização
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <param name="configuration">Configuração da aplicação</param>
    /// <returns>Coleção de serviços configurada</returns>
    private static IServiceCollection AddJwtConfigurationValidation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtConfigurationOptions>()
            .Bind(configuration.GetSection("Jwt"))
            .ValidateDataAnnotations()
            .Validate(options =>
            {
                // Validar força da chave secreta
                if (string.IsNullOrWhiteSpace(options.Secret))
                    return false;

                if (options.Secret.Length < 32)
                    return false;

                // Validar issuer
                if (string.IsNullOrWhiteSpace(options.Issuer))
                    return false;

                // Validar audience
                if (string.IsNullOrWhiteSpace(options.Audience))
                    return false;

                // Validar configurações de expiração
                if (options.ExpirationInMinutes <= 0 || options.ExpirationInMinutes > 1440) // Max 24 horas
                    return false;

                return true;
            }, "Configuração JWT inválida. Verifique as propriedades na seção 'Jwt' do appsettings.json: 'Secret' (mínimo 32 caracteres), 'Issuer', 'Audience' e 'ExpirationInMinutes' (1-1440).");

        return services;
    }

    /// <summary>
    /// Configura o pipeline de middleware de segurança na aplicação
    /// </summary>
    /// <param name="app">Application builder</param>
    /// <returns>Application builder configurado</returns>
    public static IApplicationBuilder UseApiSecurity(this IApplicationBuilder app)
    {
        app.UseSession();

        // Middleware de segurança (tratamento de erros e logging)
        app.UseSecurityMiddleware();

        // Autenticação e autorização
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// Valida a configuração de segurança na inicialização da aplicação
    /// </summary>
    /// <param name="services">Service provider</param>
    /// <param name="logger">Logger para registrar resultados da validação</param>
    /// <returns>True se a configuração é válida, false caso contrário</returns>
    public static bool ValidateSecurityConfiguration(IServiceProvider services, ILogger logger)
    {
        try
        {
            // Validar configuração JWT
            var jwtOptions = services.GetRequiredService<Microsoft.Extensions.Options.IOptions<JwtConfigurationOptions>>();
            var config = jwtOptions.Value;

            if (string.IsNullOrWhiteSpace(config.Secret))
            {
                logger.LogError("Configuração JWT inválida: A propriedade 'Jwt.Secret' não está configurada ou está vazia. " +
                    "Verifique se a seção 'Jwt' existe no appsettings.json e contém a propriedade 'Secret' com valor válido.");
                return false;
            }

            if (config.Secret.Length < 32)
            {
                logger.LogError("Configuração JWT inválida: A propriedade 'Jwt.Secret' deve ter pelo menos 32 caracteres para garantir segurança adequada. " +
                    "Valor atual tem {Length} caracteres. Atualize a propriedade 'Jwt.Secret' no appsettings.json.", config.Secret.Length);
                return false;
            }

            if (string.IsNullOrWhiteSpace(config.Issuer))
            {
                logger.LogError("Configuração JWT inválida: A propriedade 'Jwt.Issuer' não está configurada ou está vazia. " +
                    "Verifique se a propriedade 'Issuer' existe na seção 'Jwt' do appsettings.json.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(config.Audience))
            {
                logger.LogError("Configuração JWT inválida: A propriedade 'Jwt.Audience' não está configurada ou está vazia. " +
                    "Verifique se a propriedade 'Audience' existe na seção 'Jwt' do appsettings.json.");
                return false;
            }

            if (config.ExpirationInMinutes <= 0 || config.ExpirationInMinutes > 1440)
            {
                logger.LogError("Configuração JWT inválida: A propriedade 'Jwt.ExpirationInMinutes' deve estar entre 1 e 1440 minutos. " +
                    "Valor atual: {ExpirationInMinutes}. Verifique a propriedade 'ExpirationInMinutes' na seção 'Jwt' do appsettings.json.", 
                    config.ExpirationInMinutes);
                return false;
            }

            // Validar se os serviços de autorização estão registrados
            var authService = services.GetService<IAuthorizationService>();
            if (authService == null)
            {
                logger.LogError("Configuração de segurança inválida: Serviço de autorização não foi registrado corretamente. " +
                    "Verifique se AddSystemAuthorization() foi chamado durante a configuração dos serviços.");
                return false;
            }

            logger.LogInformation("Configuração de segurança validada com sucesso. JWT configurado com: " +
                "Issuer='{Issuer}', Audience='{Audience}', ExpirationInMinutes={ExpirationInMinutes}", 
                config.Issuer, config.Audience, config.ExpirationInMinutes);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro crítico ao validar configuração de segurança. " +
                "Verifique se a seção 'Jwt' está corretamente configurada no appsettings.json e se todos os serviços necessários foram registrados.");
            return false;
        }
    }
}

/// <summary>
/// Opções de configuração JWT para validação
/// </summary>
public class JwtConfigurationOptions
{
    /// <summary>
    /// Chave secreta para assinatura dos tokens
    /// </summary>
    [Required(ErrorMessage = "A propriedade 'Jwt.Secret' é obrigatória no appsettings.json")]
    [MinLength(32, ErrorMessage = "A propriedade 'Jwt.Secret' deve ter pelo menos 32 caracteres no appsettings.json")]
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Emissor dos tokens
    /// </summary>
    [Required(ErrorMessage = "A propriedade 'Jwt.Issuer' é obrigatória no appsettings.json")]
    public string Issuer { get; set; } = "SistemaGestaoEscolar";

    /// <summary>
    /// Audiência dos tokens
    /// </summary>
    [Required(ErrorMessage = "A propriedade 'Jwt.Audience' é obrigatória no appsettings.json")]
    public string Audience { get; set; } = "SistemaGestaoEscolar.API";

    /// <summary>
    /// Tempo de expiração em minutos
    /// </summary>
    [Range(1, 1440, ErrorMessage = "A propriedade 'Jwt.ExpirationInMinutes' deve estar entre 1 e 1440 minutos no appsettings.json")]
    public int ExpirationInMinutes { get; set; } = 60;

    /// <summary>
    /// Tempo de expiração do refresh token em dias
    /// </summary>
    [Range(1, 30, ErrorMessage = "A propriedade 'Jwt.RefreshExpirationDays' deve estar entre 1 e 30 dias no appsettings.json")]
    public int RefreshExpirationDays { get; set; } = 7;

    /// <summary>
    /// Chave secreta para tokens de reset de senha
    /// </summary>
    public string PasswordResetSecret { get; set; } = string.Empty;

    /// <summary>
    /// Chave secreta para tokens de confirmação de email
    /// </summary>
    public string EmailConfirmationSecret { get; set; } = string.Empty;
}