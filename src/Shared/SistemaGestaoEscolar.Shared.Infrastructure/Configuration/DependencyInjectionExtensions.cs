using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SistemaGestaoEscolar.Shared.Infrastructure.Services;

namespace SistemaGestaoEscolar.Shared.Infrastructure.Configuration;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        // Add logging service
        services.AddScoped<ILoggingService, LoggingService>();

        // Add FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjectionExtensions).Assembly);

        return services;
    }
}