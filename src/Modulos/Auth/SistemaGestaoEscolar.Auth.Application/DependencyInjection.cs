using Microsoft.Extensions.DependencyInjection;
using SistemaGestaoEscolar.Auth.Application.Interfaces;
using SistemaGestaoEscolar.Auth.Application.Services;
using SistemaGestaoEscolar.Auth.Application.UseCases;

namespace SistemaGestaoEscolar.Auth.Application;

/// <summary>
/// Configuração de injeção de dependência para a camada de aplicação de autenticação.
/// Registra todos os serviços e casos de uso necessários.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adiciona os serviços da camada de aplicação de autenticação
    /// </summary>
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        // Registrar casos de uso
        services.AddScoped<LoginUseCase>();
        services.AddScoped<LogoutUseCase>();
        services.AddScoped<RefreshTokenUseCase>();
        services.AddScoped<ChangePasswordUseCase>();
        services.AddScoped<ForgotPasswordUseCase>();
        services.AddScoped<ResetPasswordUseCase>();
        services.AddScoped<ConfirmEmailUseCase>();
        services.AddScoped<ResendEmailConfirmationUseCase>();

        // Registrar serviços de aplicação
        services.AddScoped<IAuthApplicationService, AuthApplicationService>();
        services.AddScoped<AuthValidationService>();

        return services;
    }
}