using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SistemaGestaoEscolar.Professores.Aplicacao.Commands;
using SistemaGestaoEscolar.Professores.Dominio.Repositorios;
using SistemaGestaoEscolar.Professores.Dominio.Servicos;
using SistemaGestaoEscolar.Professores.Infraestrutura.Repositorios;
using SistemaGestaoEscolar.Professores.Infraestrutura.Servicos;
using SistemaGestaoEscolar.Professores.Infraestrutura.Validadores;

namespace SistemaGestaoEscolar.Professores.Infraestrutura.Configuracao;

public static class ProfessoresModuleConfiguration
{
    public static IServiceCollection AddProfessoresModule(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IRepositorioProfessor, RepositorioProfessorMySql>();
        services.AddScoped<IReadModelRepositoryProfessor, ReadModelRepositoryProfessor>();

        // Domain Services
        services.AddScoped<IServicosDominioProfessor, ServicosDominioProfessor>();
        services.AddScoped<IServicoIntegracaoDisciplina, ServicoIntegracaoDisciplina>();

        // MediatR - Command Handlers e Query Handlers
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(CriarProfessorCommand).Assembly);
        });

        // Validators
        services.AddValidatorsFromAssemblyContaining<CriarProfessorCommandValidator>();
        services.AddScoped<IValidator<CriarProfessorCommand>, CriarProfessorCommandValidator>();
        services.AddScoped<IValidator<AtualizarProfessorCommand>, AtualizarProfessorCommandValidator>();
        services.AddScoped<IValidator<AtribuirDisciplinaCommand>, AtribuirDisciplinaCommandValidator>();
        services.AddScoped<IValidator<RemoverDisciplinaCommand>, RemoverDisciplinaCommandValidator>();
        services.AddScoped<IValidator<DesativarProfessorCommand>, DesativarProfessorCommandValidator>();

        return services;
    }
}