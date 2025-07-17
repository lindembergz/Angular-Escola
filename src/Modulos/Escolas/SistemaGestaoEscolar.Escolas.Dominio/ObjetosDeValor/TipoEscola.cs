using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;

public class TipoEscola : ValueObject
{
    public string Valor { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public int IdadeMinima { get; private set; }
    public int IdadeMaxima { get; private set; }

    private TipoEscola() { } // Para EF Core

    public TipoEscola(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Tipo de escola não pode ser vazio", nameof(valor));

        var tipoValido = ObterTipoValido(valor);
        if (tipoValido == null)
            throw new ArgumentException($"Tipo de escola inválido: {valor}", nameof(valor));

        Valor = tipoValido.Valor;
        Descricao = tipoValido.Descricao;
        IdadeMinima = tipoValido.IdadeMinima;
        IdadeMaxima = tipoValido.IdadeMaxima;
    }

    private static TipoEscolaInfo? ObterTipoValido(string valor)
    {
        var tiposValidos = new Dictionary<string, TipoEscolaInfo>(StringComparer.OrdinalIgnoreCase)
        {
            ["Infantil"] = new("Infantil", "Educação Infantil", 0, 5),
            ["Fundamental"] = new("Fundamental", "Ensino Fundamental", 6, 14),
            ["Médio"] = new("Médio", "Ensino Médio", 15, 17),
            ["Fundamental e Médio"] = new("Fundamental e Médio", "Ensino Fundamental e Médio", 6, 17),
            ["Técnico"] = new("Técnico", "Ensino Técnico", 16, 25),
            ["EJA"] = new("EJA", "Educação de Jovens e Adultos", 18, 99),
            ["Superior"] = new("Superior", "Ensino Superior", 18, 99),
            ["Pós-Graduação"] = new("Pós-Graduação", "Pós-Graduação", 21, 99)
        };

        return tiposValidos.TryGetValue(valor, out var tipo) ? tipo : null;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor;
    }

    public override string ToString() => Descricao;

    public static implicit operator string(TipoEscola tipoEscola) => tipoEscola?.Valor ?? string.Empty;
    public static implicit operator TipoEscola(string valor) => new(valor);

    // Tipos pré-definidos para facilitar o uso
    public static TipoEscola Infantil => new("Infantil");
    public static TipoEscola Fundamental => new("Fundamental");
    public static TipoEscola Medio => new("Médio");
    public static TipoEscola FundamentalEMedio => new("Fundamental e Médio");
    public static TipoEscola Tecnico => new("Técnico");
    public static TipoEscola EJA => new("EJA");
    public static TipoEscola Superior => new("Superior");
    public static TipoEscola PosGraduacao => new("Pós-Graduação");

    public static IEnumerable<TipoEscola> ObterTodosOsTipos()
    {
        yield return Infantil;
        yield return Fundamental;
        yield return Medio;
        yield return FundamentalEMedio;
        yield return Tecnico;
        yield return EJA;
        yield return Superior;
        yield return PosGraduacao;
    }

    public bool EhEducacaoBasica()
    {
        return Valor is "Infantil" or "Fundamental" or "Médio" or "Fundamental e Médio";
    }

    public bool EhEnsinoSuperior()
    {
        return Valor is "Superior" or "Pós-Graduação";
    }

    public bool EhEnsinoTecnico()
    {
        return Valor == "Técnico";
    }

    public bool EhEJA()
    {
        return Valor == "EJA";
    }

    public bool AtendeFaixaEtaria(int idade)
    {
        return idade >= IdadeMinima && idade <= IdadeMaxima;
    }

    public bool PodeMatricularIdade(int idade)
    {
        // Para EJA e Superior, a idade mínima é mais flexível
        if (Valor is "EJA" or "Superior" or "Pós-Graduação")
            return idade >= IdadeMinima;

        return AtendeFaixaEtaria(idade);
    }

    public string ObterNivelEnsino()
    {
        return Valor switch
        {
            "Infantil" => "Educação Infantil",
            "Fundamental" or "Fundamental e Médio" => "Ensino Fundamental",
            "Médio" => "Ensino Médio",
            "Técnico" => "Educação Profissional",
            "EJA" => "Educação de Jovens e Adultos",
            "Superior" => "Educação Superior",
            "Pós-Graduação" => "Pós-Graduação",
            _ => "Não definido"
        };
    }

    public IEnumerable<string> ObterSeriesEsperadas()
    {
        return Valor switch
        {
            "Infantil" => new[] { "Berçário I", "Berçário II", "Maternal I", "Maternal II", "Pré I", "Pré II" },
            "Fundamental" => new[] { "1º Ano", "2º Ano", "3º Ano", "4º Ano", "5º Ano", "6º Ano", "7º Ano", "8º Ano", "9º Ano" },
            "Médio" => new[] { "1º Ano", "2º Ano", "3º Ano" },
            "Fundamental e Médio" => new[] { "1º Ano Fund", "2º Ano Fund", "3º Ano Fund", "4º Ano Fund", "5º Ano Fund", 
                                           "6º Ano Fund", "7º Ano Fund", "8º Ano Fund", "9º Ano Fund",
                                           "1º Ano Médio", "2º Ano Médio", "3º Ano Médio" },
            "Técnico" => new[] { "1º Módulo", "2º Módulo", "3º Módulo", "4º Módulo" },
            "EJA" => new[] { "EJA I", "EJA II", "EJA III", "EJA IV" },
            _ => Array.Empty<string>()
        };
    }

    private record TipoEscolaInfo(string Valor, string Descricao, int IdadeMinima, int IdadeMaxima);
}