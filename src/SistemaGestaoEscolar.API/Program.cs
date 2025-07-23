using Microsoft.EntityFrameworkCore;
using Serilog;
using SistemaGestaoEscolar.API.Configuration;
using SistemaGestaoEscolar.API.Middleware;
using SistemaGestaoEscolar.Alunos.Infraestrutura.Configuracao;
using SistemaGestaoEscolar.Auth.Application;
using SistemaGestaoEscolar.Auth.Infrastructure;
using SistemaGestaoEscolar.Escolas.Infraestrutura.Configuracao;
using SistemaGestaoEscolar.Shared.Infrastructure.Configuration;
using SistemaGestaoEscolar.Shared.Infrastructure.Middleware;
using SistemaGestaoEscolar.Shared.Infrastructure.Authorization;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/sistema-gestao-escolar-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Iniciando Sistema de Gestão Escolar API");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { 
            Title = "Sistema de Gestão Escolar API", 
            Version = "v1",
            Description = "API para Sistema de Gestão Escolar com arquitetura modular"
        });
        
        // Resolver conflitos de nomes de DTOs entre módulos
        c.CustomSchemaIds(type => 
        {
            var typeName = type.Name;
            var namespaceParts = type.Namespace?.Split('.') ?? Array.Empty<string>();
            
            // Se for um DTO de um módulo específico, incluir o nome do módulo no schema ID
            // Formato: SistemaGestaoEscolar.{Modulo}.Aplicacao.{DTOs|Queries|Commands}
            if (namespaceParts.Length >= 3 && namespaceParts[0] == "SistemaGestaoEscolar")
            {
                var moduleName = namespaceParts[1]; // Ex: "Alunos", "Escolas", "Auth"
                
                // Verificar se é um módulo conhecido (não Shared)
                if (moduleName != "Shared" && moduleName != "API")
                {
                    return $"{moduleName}{typeName}";
                }
            }
            
            return typeName;
        });
    });

    // Configure Database
    builder.Services.AddDatabaseConfiguration(builder.Configuration, builder.Environment);

    // Configure CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    // Configure Health Checks
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<ApplicationDbContext>();

    // Configure centralized API security (JWT, Authorization, Middleware)
    builder.Services.AddApiSecurity(builder.Configuration);

    // Configure shared infrastructure
    builder.Services.AddSharedInfrastructure();

    // Configure modules
    builder.Services.AddAuthApplication();
    builder.Services.AddAuthInfrastructure(builder.Configuration);
    builder.Services.AdicionarModuloEscolas(builder.Configuration);
    builder.Services.AddCompleteAlunosModule();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema de Gestão Escolar API v1");
            c.RoutePrefix = string.Empty; // Swagger UI at root
        });
    }

    // Add custom middleware in correct order
    app.UseMiddleware<ValidationMiddleware>();
    app.UseMiddleware<ErrorHandlingMiddleware>();

    app.UseHttpsRedirection();
    app.UseCors("AllowAngularApp");
    
    app.UseAuthInfrastructure();

    // Configure centralized security middleware pipeline
    app.UseApiSecurity();

    app.MapControllers();
    app.MapHealthChecks("/health");

    // Initialize database (skip in test environment)
    if (!app.Environment.IsEnvironment("Testing"))
    {
        await DatabaseConfiguration.InitializeDatabaseAsync(app.Services, app.Environment);
    }

    // Validate security configuration (skip in test environment)
    if (!app.Environment.IsEnvironment("Testing"))
    {
        using (var scope = app.Services.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            if (!SecurityConfiguration.ValidateSecurityConfiguration(scope.ServiceProvider, logger))
            {
                Log.Fatal("Configuração de segurança inválida. Aplicação não pode iniciar.");
                return;
            }
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação falhou ao iniciar");
}
finally
{
    Log.CloseAndFlush();
}

// Make Program class accessible for testing
public partial class Program { }
