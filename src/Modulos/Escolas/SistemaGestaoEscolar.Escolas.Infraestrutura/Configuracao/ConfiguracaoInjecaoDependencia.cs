using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using SistemaGestaoEscolar.Escolas.Aplicacao.CasosDeUso;
using SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;
using SistemaGestaoEscolar.Escolas.Aplicacao.Servicos;
using SistemaGestaoEscolar.Escolas.Dominio.Repositorios;
using SistemaGestaoEscolar.Escolas.Dominio.Servicos;
using SistemaGestaoEscolar.Escolas.Infraestrutura.Contexto;
using SistemaGestaoEscolar.Escolas.Infraestrutura.Repositorios;

using SistemaGestaoEscolar.Escolas.Infraestrutura.Validadores;

namespace SistemaGestaoEscolar.Escolas.Infraestrutura.Configuracao;

public static class ConfiguracaoInjecaoDependencia
{
    public static IServiceCollection AdicionarModuloEscolas(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuração do DbContext
        services.AddDbContext<EscolasDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            // Use in-memory database for development
            if (string.IsNullOrEmpty(connectionString) || configuration.GetValue<bool>("Development"))
            {
                options.UseInMemoryDatabase("SistemaGestaoEscolar_Escolas_Dev");
            }
            else
            {
                options.UseMySql(connectionString, ServerVersion.Parse("8.0.0"));
            }
            
            // Configurações adicionais para desenvolvimento
            if (configuration.GetValue<bool>("Development"))
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // Repositórios
        services.AddScoped<IRepositorioEscola, RepositorioEscolaMySql>();
        services.AddScoped<IRepositorioRedeEscolar, RepositorioRedeEscolarMySql>();

        // Serviços de Domínio
        services.AddScoped<IServicosDominioEscola, SistemaGestaoEscolar.Escolas.Dominio.Servicos.ServicosDominioEscola>();

        // Casos de Uso
        services.AddScoped<CriarEscolaCasoDeUso>();
        services.AddScoped<CriarRedeEscolarCasoDeUso>();
        services.AddScoped<AdicionarUnidadeEscolarCasoDeUso>();
        services.AddScoped<ObterEscolasPorRedeCasoDeUso>();

        // Serviços de Aplicação
        services.AddScoped<IServicoAplicacaoEscola, ServicoAplicacaoEscola>();

        // Validadores FluentValidation
        services.AddScoped<IValidator<CriarEscolaDto>, ValidadorCriarEscola>();
        services.AddScoped<IValidator<CriarRedeEscolarDto>, ValidadorCriarRedeEscolar>();
        services.AddScoped<IValidator<EnderecoDto>, ValidadorEndereco>();
        services.AddScoped<IValidator<AdicionarUnidadeEscolarDto>, ValidadorAdicionarUnidadeEscolar>();

        // Configuração do FluentValidation
        services.AddValidatorsFromAssemblyContaining<ValidadorCriarEscola>();

        return services;
    }

    public static IServiceCollection AdicionarValidadoresEscolas(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CriarEscolaDto>, ValidadorCriarEscola>();
        services.AddScoped<IValidator<CriarRedeEscolarDto>, ValidadorCriarRedeEscolar>();
        services.AddScoped<IValidator<EnderecoDto>, ValidadorEndereco>();
        services.AddScoped<IValidator<AdicionarUnidadeEscolarDto>, ValidadorAdicionarUnidadeEscolar>();

        return services;
    }

    public static IServiceCollection AdicionarRepositoriosEscolas(this IServiceCollection services)
    {
        services.AddScoped<IRepositorioEscola, RepositorioEscolaMySql>();
        services.AddScoped<IRepositorioRedeEscolar, RepositorioRedeEscolarMySql>();

        return services;
    }

    public static IServiceCollection AdicionarServicosEscolas(this IServiceCollection services)
    {
        services.AddScoped<IServicosDominioEscola, SistemaGestaoEscolar.Escolas.Dominio.Servicos.ServicosDominioEscola>();
        services.AddScoped<IServicoAplicacaoEscola, ServicoAplicacaoEscola>();

        return services;
    }
}