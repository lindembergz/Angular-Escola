using Microsoft.EntityFrameworkCore;
using Serilog;
using SistemaGestaoEscolar.API.Configuration;
using SistemaGestaoEscolar.API.Middleware;
using SistemaGestaoEscolar.Auth.Application;
using SistemaGestaoEscolar.Auth.Infrastructure;
using SistemaGestaoEscolar.Escolas.Infraestrutura.Configuracao;
using SistemaGestaoEscolar.Shared.Infrastructure.Configuration;
using SistemaGestaoEscolar.Shared.Infrastructure.Middleware;

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

    // Configure shared infrastructure
    builder.Services.AddSharedInfrastructure();

    // Configure modules
    builder.Services.AddAuthApplication();
    builder.Services.AddAuthInfrastructure(builder.Configuration);
    builder.Services.AdicionarModuloEscolas(builder.Configuration);

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

    // Add custom middleware
    app.UseMiddleware<ValidationMiddleware>();
    app.UseMiddleware<ErrorHandlingMiddleware>();

    app.UseHttpsRedirection();
    app.UseCors("AllowAngularApp");

    app.UseAuthInfrastructure();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health");

    // Initialize database
    await DatabaseConfiguration.InitializeDatabaseAsync(app.Services, app.Environment);

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
