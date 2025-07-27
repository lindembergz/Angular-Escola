using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SistemaGestaoEscolar.Academico.Aplicacao.Commands;
using SistemaGestaoEscolar.Academico.Aplicacao.Interfaces;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;
using SistemaGestaoEscolar.Academico.Dominio.Servicos;
using SistemaGestaoEscolar.Academico.Infraestrutura.Repositorios;
using SistemaGestaoEscolar.Academico.Infraestrutura.Servicos;
using SistemaGestaoEscolar.Academico.Infraestrutura.Validadores;

namespace SistemaGestaoEscolar.Academico.Infraestrutura.Configuracao;

public static class AcademicoModuleConfiguration
{
    public static IServiceCollection AddAcademicoModule(this IServiceCollection services)
    {
        // Registrar repositórios Write (MySQL)
        services.AddScoped<IRepositorioTurma, RepositorioTurmaMySql>();
        services.AddScoped<IRepositorioDisciplina, RepositorioDisciplinaMySql>();
        services.AddScoped<IRepositorioHorario, RepositorioHorarioMySql>();

        // Registrar repositório Read (para CQRS)
        services.AddScoped<ReadModelRepositoryAcademico>();
        
        // Registrar serviços de query
        services.AddScoped<ITurmaQueryService, TurmaQueryService>();

        // Registrar serviços de domínio
        services.AddScoped<IServicosDominioTurma, ServicosDominioTurma>();
        services.AddScoped<IServicosDominioHorario, ServicosDominioHorario>();

        // MediatR - Command Handlers e Query Handlers
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(CriarTurmaCommand).Assembly);
        });

        // Registrar validadores
        services.AddScoped<IValidator<CriarTurmaCommand>, CriarTurmaCommandValidator>();
        services.AddScoped<IValidator<CriarDisciplinaCommand>, CriarDisciplinaCommandValidator>();
        services.AddScoped<IValidator<CriarHorarioCommand>, CriarHorarioCommandValidator>();

        return services;
    }

    public static IServiceCollection AddCompleteAcademicoModule(this IServiceCollection services)
    {
        return services.AddAcademicoModule();
    }
}