using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Professores.Dominio.ObjetosDeValor;

public class TituloAcademico : ValueObject
{
    public TipoTitulo Tipo { get; private set; }
    public string Curso { get; private set; } = string.Empty;
    public string Instituicao { get; private set; } = string.Empty;
    public int AnoFormatura { get; private set; }
    public string Descricao => ObterDescricao();

    private TituloAcademico() { } // Para EF Core

    public TituloAcademico(TipoTitulo tipo, string curso, string instituicao, int anoFormatura)
    {
        if (string.IsNullOrWhiteSpace(curso))
            throw new ArgumentException("Curso não pode ser vazio", nameof(curso));

        if (string.IsNullOrWhiteSpace(instituicao))
            throw new ArgumentException("Instituição não pode ser vazia", nameof(instituicao));

        if (anoFormatura < 1950 || anoFormatura > DateTime.Now.Year)
            throw new ArgumentException("Ano de formatura inválido", nameof(anoFormatura));

        Tipo = tipo;
        Curso = curso.Trim();
        Instituicao = instituicao.Trim();
        AnoFormatura = anoFormatura;
    }

    private string ObterDescricao()
    {
        var tipoDescricao = Tipo switch
        {
            TipoTitulo.EnsinoMedio => "Ensino Médio",
            TipoTitulo.Tecnologo => "Tecnólogo",
            TipoTitulo.Graduacao => "Graduação",
            TipoTitulo.PosGraduacao => "Pós-Graduação",
            TipoTitulo.Mestrado => "Mestrado",
            TipoTitulo.Doutorado => "Doutorado",
            TipoTitulo.PosDoutorado => "Pós-Doutorado",
            _ => "Não Informado"
        };

        return $"{tipoDescricao} em {Curso} - {Instituicao} ({AnoFormatura})";
    }

    public bool EhTituloSuperior()
    {
        return Tipo >= TipoTitulo.Graduacao;
    }

    public bool EhTituloPosGraduacao()
    {
        return Tipo >= TipoTitulo.PosGraduacao;
    }

    public int ObterNivelAcademico()
    {
        return (int)Tipo;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Tipo;
        yield return Curso.ToLowerInvariant();
        yield return Instituicao.ToLowerInvariant();
        yield return AnoFormatura;
    }

    public override string ToString() => Descricao;
}

public enum TipoTitulo
{
    EnsinoMedio = 1,
    Tecnologo = 2,
    Graduacao = 3,
    PosGraduacao = 4,
    Mestrado = 5,
    Doutorado = 6,
    PosDoutorado = 7
}