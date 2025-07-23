using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace SistemaGestaoEscolar.Shared.Infrastructure.Authorization;

/// <summary>
/// Configuração centralizada de autorização para o sistema.
/// Define todas as políticas de autorização baseadas em roles e claims.
/// </summary>
public static class AuthorizationConfiguration
{
    /// <summary>
    /// Configura todas as políticas de autorização do sistema
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <returns>Coleção de serviços configurada</returns>
    public static IServiceCollection AddSystemAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Políticas baseadas em roles hierárquicas
            ConfigureRoleBasedPolicies(options);

            // Políticas baseadas em claims específicos
            ConfigureClaimBasedPolicies(options);

            // Políticas compostas e específicas
            ConfigureCompositePolicies(options);
        });

        return services;
    }

    /// <summary>
    /// Configura políticas baseadas em roles com hierarquia
    /// </summary>
    private static void ConfigureRoleBasedPolicies(AuthorizationOptions options)
    {
        // Super Admin - acesso total
        options.AddPolicy(AuthorizationPolicies.SuperAdmin, policy =>
            policy.RequireRole("SuperAdmin"));

        // Admin - inclui SuperAdmin
        options.AddPolicy(AuthorizationPolicies.Admin, policy =>
            policy.RequireRole("SuperAdmin", "Admin"));

        // Director - inclui Admin e SuperAdmin
        options.AddPolicy(AuthorizationPolicies.Director, policy =>
            policy.RequireRole("SuperAdmin", "Admin", "Director"));

        // Coordinator - inclui níveis superiores
        options.AddPolicy(AuthorizationPolicies.Coordinator, policy =>
            policy.RequireRole("SuperAdmin", "Admin", "Director", "Coordinator"));

        // Secretary - inclui níveis superiores
        options.AddPolicy(AuthorizationPolicies.Secretary, policy =>
            policy.RequireRole("SuperAdmin", "Admin", "Director", "Coordinator", "Secretary"));

        // Teacher - inclui níveis superiores
        options.AddPolicy(AuthorizationPolicies.Teacher, policy =>
            policy.RequireRole("SuperAdmin", "Admin", "Director", "Coordinator", "Secretary", "Teacher"));

        // SchoolStaff - todos os funcionários da escola
        options.AddPolicy(AuthorizationPolicies.SchoolStaff, policy =>
            policy.RequireRole("SuperAdmin", "Admin", "Director", "Coordinator", "Secretary", "Teacher"));
    }

    /// <summary>
    /// Configura políticas baseadas em claims específicos
    /// </summary>
    private static void ConfigureClaimBasedPolicies(AuthorizationOptions options)
    {
        // Acesso acadêmico
        options.AddPolicy(AuthorizationPolicies.AcademicAccess, policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("SuperAdmin") ||
                context.User.IsInRole("Admin") ||
                context.User.IsInRole("Director") ||
                context.User.IsInRole("Coordinator") ||
                context.User.IsInRole("Teacher") ||
                context.User.HasClaim("permission", "academic.read")));

        // Acesso financeiro
        options.AddPolicy(AuthorizationPolicies.FinancialAccess, policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("SuperAdmin") ||
                context.User.IsInRole("Admin") ||
                context.User.IsInRole("Director") ||
                context.User.HasClaim("permission", "financial.read")));

        // Geração de relatórios
        options.AddPolicy(AuthorizationPolicies.ReportsAccess, policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("SuperAdmin") ||
                context.User.IsInRole("Admin") ||
                context.User.IsInRole("Director") ||
                context.User.IsInRole("Coordinator") ||
                context.User.HasClaim("permission", "reports.generate")));

        // Configuração do sistema
        options.AddPolicy(AuthorizationPolicies.SystemConfig, policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("SuperAdmin") ||
                context.User.HasClaim("permission", "system.configure")));
    }

    /// <summary>
    /// Configura políticas compostas e específicas
    /// </summary>
    private static void ConfigureCompositePolicies(AuthorizationOptions options)
    {
        // Email confirmado
        options.AddPolicy(AuthorizationPolicies.EmailConfirmed, policy =>
            policy.RequireClaim("email_confirmed", "true"));

        // Acesso específico à escola
        options.AddPolicy(AuthorizationPolicies.SchoolAccess, policy =>
            policy.RequireAssertion(context =>
            {
                // SuperAdmin e Admin têm acesso a todas as escolas
                if (context.User.IsInRole("SuperAdmin") || context.User.IsInRole("Admin"))
                    return true;

                // Outros usuários precisam ter school_id claim
                var schoolIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "school_id");
                return schoolIdClaim != null && !string.IsNullOrWhiteSpace(schoolIdClaim.Value);
            }));

        // Gerenciamento de usuários
        options.AddPolicy(AuthorizationPolicies.UserManagement, policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("SuperAdmin") ||
                context.User.IsInRole("Admin") ||
                (context.User.IsInRole("Director") && context.User.Claims.Any(c => c.Type == "school_id"))));

        // Gerenciamento de alunos
        options.AddPolicy(AuthorizationPolicies.StudentManagement, policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("SuperAdmin") ||
                context.User.IsInRole("Admin") ||
                context.User.IsInRole("Director") ||
                context.User.IsInRole("Coordinator") ||
                context.User.IsInRole("Secretary") ||
                context.User.IsInRole("Teacher")));

        // Gerenciamento de escolas
        options.AddPolicy(AuthorizationPolicies.SchoolManagement, policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("SuperAdmin") ||
                context.User.IsInRole("Admin") ||
                (context.User.IsInRole("Director") && context.User.Claims.Any(c => c.Type == "school_id"))));

        // Gerenciamento de redes escolares
        options.AddPolicy(AuthorizationPolicies.NetworkManagement, policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("SuperAdmin") ||
                context.User.IsInRole("Admin")));
    }
}