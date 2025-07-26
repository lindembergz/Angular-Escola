using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;

public class Serie : ValueObject
{
    public TipoSerie Tipo { get; private set; }
    public int Ano { get; private set; }
    public string Descricao { get; private set; }

    private Serie(TipoSerie tipo, int ano, string descricao)
    {
        Tipo = tipo;
        Ano = ano;
        Descricao = descricao;
    }

    public static Serie Criar(TipoSerie tipo, int ano)
    {
        ValidarAno(tipo, ano);
        var descricao = ObterDescricao(tipo, ano);
        return new Serie(tipo, ano, descricao);
    }

    private static void ValidarAno(TipoSerie tipo, int ano)
    {
        switch (tipo)
        {
            case TipoSerie.Infantil when ano < 1 || ano > 5:
                throw new ArgumentException("Educação Infantil deve ter ano entre 1 e 5");
            case TipoSerie.Fundamental when ano < 1 || ano > 9:
                throw new ArgumentException("Ensino Fundamental deve ter ano entre 1 e 9");
            case TipoSerie.Medio when ano < 1 || ano > 3:
                throw new ArgumentException("Ensino Médio deve ter ano entre 1 e 3");
        }
    }

    private static string ObterDescricao(TipoSerie tipo, int ano)
    {
        return tipo switch
        {
            TipoSerie.Infantil => $"Infantil {ano}",
            TipoSerie.Fundamental => $"{ano}º Ano Fundamental",
            TipoSerie.Medio => $"{ano}º Ano Médio",
            _ => throw new ArgumentException("Tipo de série inválido")
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Tipo;
        yield return Ano;
    }

    public override string ToString() => Descricao;
}

public enum TipoSerie
{
    Infantil = 1,
    Fundamental = 2,
    Medio = 3
}