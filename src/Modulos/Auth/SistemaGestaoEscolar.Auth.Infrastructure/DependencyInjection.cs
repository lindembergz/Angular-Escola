using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SistemaGestaoEscolar.Auth.Application.DTOs;
using SistemaGestaoEscolar.Auth.Application.Interfaces;
using SistemaGestaoEscolar.Auth.Domain.Repositories;
using SistemaGestaoEscolar.Auth.Domain.Services;
using SistemaGestaoEscolar.Auth.Infrastructure.Context;
using SistemaGestaoEscolar.Auth.Infrastructure.Repositories;
using SistemaGestaoEscolar.Auth.Infrastructure.Services;
using SistemaGestaoEscolar.Auth.Infrastructure.Validators;
using System.Text;

namespace SistemaGestaoEscolar.Auth.Infrastructure;

/// <summary>
/// Configuração de injeção de dependência para a camada de infraestrutura de autenticação.
/// Registra todos os serviços, repositórios e configurações necessárias.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adiciona os serviços da camada de infraestrutura de autenticação
    /// </summary>
    public static IServiceCollection AddAuthInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Configurar Entity Framework
        services.AddDbContext<AuthDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mysqlOptions =>
            {
                mysqlOptions.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName);
                mysqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

            // Configurações adicionais para desenvolvimento
            if (configuration.GetValue<bool>("Development:EnableSensitiveDataLogging"))
            {
                options.EnableSensitiveDataLogging();
            }

            if (configuration.GetValue<bool>("Development:EnableDetailedErrors"))
            {
                options.EnableDetailedErrors();
            }
        });

        // Registrar repositórios
        services.AddScoped<IUserRepository, MySqlUserRepository>();
        services.AddScoped<ISessionRepository, MySqlSessionRepository>();

        // Registrar serviços de domínio
        services.AddScoped<IAuthDomainService, AuthDomainService>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();

        // Registrar serviços de aplicação
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<DataSeedService>();

        // Registrar validadores
        services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
        services.AddScoped<IValidator<RefreshTokenDto>, RefreshTokenDtoValidator>();
        services.AddScoped<IValidator<ChangePasswordDto>, ChangePasswordDtoValidator>();
        services.AddScoped<IValidator<ForgotPasswordDto>, ForgotPasswordDtoValidator>();
        services.AddScoped<IValidator<ResetPasswordDto>, ResetPasswordDtoValidator>();

        // Nota: A configuração de autenticação JWT foi movida para SecurityConfiguration (shared infrastructure)
        // para evitar duplicação e centralizar a configuração de segurança

        // Configurar cache para sessões (se Redis estiver configurado)
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "SistemaGestaoEscolar.Auth";
            });
        }
        else
        {
            // Fallback para cache em memória
            services.AddMemoryCache();
        }

        // Configurar CORS para autenticação
        services.AddCors(options =>
        {
            options.AddPolicy("AuthPolicy", builder =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                    ?? new[] { "http://localhost:4200", "https://localhost:4200" };

                builder
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
            });
        });

        // Configurar rate limiting (proteção contra ataques de força bruta)
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

        return services;
    }

    /// <summary>
    /// Configura o middleware específico do módulo de autenticação
    /// </summary>
    public static IApplicationBuilder UseAuthInfrastructure(this IApplicationBuilder app)
    {
        // Configurar CORS específico para autenticação
        app.UseCors("AuthPolicy");
        
        // Nota: UseAuthentication() e UseAuthorization() são configurados centralmente
        // através do SecurityConfiguration.UseApiSecurity()
        
        return app;
    }
}

/// <summary>
/// Configurações para rate limiting
/// </summary>
public class IpRateLimitOptions
{
    public bool EnableEndpointRateLimiting { get; set; } = true;
    public bool StackBlockedRequests { get; set; } = false;
    public string RealIpHeader { get; set; } = "X-Real-IP";
    public string ClientIdHeader { get; set; } = "X-ClientId";
    public int HttpStatusCode { get; set; } = 429;
    public List<IpRateLimitPolicy> IpWhitelist { get; set; } = new();
    public List<IpRateLimitPolicy> GeneralRules { get; set; } = new();
}

public class IpRateLimitPolicy
{
    public string Ip { get; set; } = string.Empty;
    public List<RateLimitRule> Rules { get; set; } = new();
}

public class RateLimitRule
{
    public string Endpoint { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public int Limit { get; set; }
}