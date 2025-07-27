using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SistemaGestaoEscolar.Alunos.Aplicacao.Commands;
using SistemaGestaoEscolar.Alunos.Aplicacao.DTOs;
using SistemaGestaoEscolar.Alunos.Dominio.Repositorios;
using SistemaGestaoEscolar.Alunos.Dominio.Servicos;
using SistemaGestaoEscolar.Alunos.Infraestrutura.Repositorios;
using SistemaGestaoEscolar.Alunos.Infraestrutura.Validadores;
using System.Reflection;

namespace SistemaGestaoEscolar.Alunos.Infraestrutura.Configuracao;

public static class AlunosModuleConfiguration
{
    public static IServiceCollection AddAlunosModule(this IServiceCollection services)
    {
        // Repositórios
        services.AddScoped<IRepositorioAluno, RepositorioAlunoMySql>();
        services.AddScoped<IRepositorioMatricula, RepositorioMatriculaMySql>();

        // Serviços de Domínio
        services.AddScoped<IServicosDominioAluno, ServicosDominioAluno>();

        // MediatR - Command Handlers e Query Handlers
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(CriarAlunoCommand).Assembly);
        });

        // Validadores FluentValidation
        services.AddValidatorsFromAssemblyContaining<CriarAlunoCommandValidator>();

        // AutoMapper (se necessário)
        // services.AddAutoMapper(typeof(AlunoProfile));

        return services;
    }

    public static IServiceCollection AddAlunosValidators(this IServiceCollection services)
    {
        // Registrar validadores específicos
        services.AddScoped<IValidator<CriarAlunoCommand>, CriarAlunoCommandValidator>();
        services.AddScoped<IValidator<AtualizarAlunoCommand>, AtualizarAlunoCommandValidator>();
        services.AddScoped<IValidator<MatricularAlunoCommand>, MatricularAlunoCommandValidator>();
        services.AddScoped<IValidator<TransferirAlunoCommand>, TransferirAlunoCommandValidator>();
        services.AddScoped<IValidator<MatricularAlunoRequest>, MatricularAlunoRequestValidator>();

        return services;
    }

    public static IServiceCollection AddAlunosEventHandlers(this IServiceCollection services)
    {
        // Event Handlers são registrados automaticamente pelo MediatR
        // Mas podemos registrar serviços específicos que eles dependem

        // Exemplo: Serviços de notificação, integração, etc.
        // services.AddScoped<IEmailService, EmailService>();
        // services.AddScoped<INotificationService, NotificationService>();

        return services;
    }

    /// <summary>
    /// Configuração completa do módulo de Alunos
    /// </summary>
    public static IServiceCollection AddCompleteAlunosModule(this IServiceCollection services)
    {
        return services
            .AddAlunosModule()
            .AddAlunosValidators()
            .AddAlunosEventHandlers();
    }
}