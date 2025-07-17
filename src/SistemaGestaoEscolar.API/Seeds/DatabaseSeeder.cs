using Microsoft.EntityFrameworkCore;
using SistemaGestaoEscolar.API.Configuration;
using SistemaGestaoEscolar.Escolas.Dominio.Entidades;
using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;

namespace SistemaGestaoEscolar.API.Seeds;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Verifica se já existem dados
        if (await context.Set<RedeEscolar>().AnyAsync())
        {
            return; // Dados já foram inseridos
        }

        // Seed Redes Escolares
        var redePublica = new RedeEscolar(
            new NomeEscola("Rede Municipal de Educação"),
            new CNPJ("12345678000195"),
            new Endereco(
                "Rua da Prefeitura",
                "100",
                null,
                "Centro",
                "São Paulo",
                "SP",
                "01000000"
            )
        );

        var redePrivada = new RedeEscolar(
            new NomeEscola("Grupo Educacional Excellence"),
            new CNPJ("98765432000123"),
            new Endereco(
                "Avenida Paulista",
                "1000",
                "Sala 1001",
                "Bela Vista",
                "São Paulo",
                "SP",
                "01310100"
            )
        );

        context.Set<RedeEscolar>().AddRange(redePublica, redePrivada);

        // Seed Escolas
        var escolaPublica1 = new Escola(
            new NomeEscola("EMEF João da Silva"),
            new CNPJ("11111111000111"),
            new Endereco(
                "Rua das Flores",
                "123",
                "Apto 1",
                "Centro",
                "São Paulo",
                "SP",
                "01234567"
            ),
            TipoEscola.Fundamental,
            redePublica.Id
        );

        var escolaPublica2 = new Escola(
            new NomeEscola("EMEF Maria Santos"),
            new CNPJ("22222222000222"),
            new Endereco(
                "Avenida Brasil",
                "456",
                null,
                "Vila Nova",
                "São Paulo",
                "SP",
                "01234568"
            ),
            TipoEscola.FundamentalEMedio,
            redePublica.Id
        );

        var escolaPrivada1 = new Escola(
            new NomeEscola("Colégio Excellence"),
            new CNPJ("33333333000333"),
            new Endereco(
                "Rua dos Estudantes",
                "789",
                "Bloco A",
                "Jardins",
                "São Paulo",
                "SP",
                "01234569"
            ),
            TipoEscola.FundamentalEMedio,
            redePrivada.Id
        );

        var escolaIndependente = new Escola(
            new NomeEscola("Escola Independente Montessori"),
            new CNPJ("44444444000444"),
            new Endereco(
                "Rua da Liberdade",
                "321",
                null,
                "Liberdade",
                "São Paulo",
                "SP",
                "01234570"
            ),
            TipoEscola.Infantil,
            null // Escola independente, sem rede
        );

        context.Set<Escola>().AddRange(
            escolaPublica1,
            escolaPublica2,
            escolaPrivada1,
            escolaIndependente
        );

        await context.SaveChangesAsync();
    }
}