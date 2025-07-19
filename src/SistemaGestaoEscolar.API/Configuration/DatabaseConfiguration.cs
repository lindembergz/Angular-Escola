using Microsoft.EntityFrameworkCore;

namespace SistemaGestaoEscolar.API.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services, 
        IConfiguration configuration, 
        IWebHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            // Use MySQL for both development and production to enable migrations
            options.UseMySql(connectionString, ServerVersion.Parse("8.0.0"), mysqlOptions =>
            {
                mysqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }

            // Configure logging
            options.LogTo(Console.WriteLine, LogLevel.Information);
        });

        return services;
    }

    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider, IWebHostEnvironment environment)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            if (environment.IsDevelopment())
            {
                // Ensure in-memory database is created
                await context.Database.EnsureCreatedAsync();
                
                // Seed development data
                await SistemaGestaoEscolar.API.Seeds.DatabaseSeeder.SeedAsync(context);
            }
            else
            {
                // Apply migrations for production
                await context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            logger.LogError(ex, "Erro ao inicializar o banco de dados");
            throw;
        }
    }
}