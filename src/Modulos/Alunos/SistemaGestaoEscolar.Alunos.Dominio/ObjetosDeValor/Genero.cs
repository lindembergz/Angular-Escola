using SistemaGestaoEscolar.Shared.Domain.ValueObjects;
using SistemaGestaoEscolar.Shared.Domain.Exceptions;

namespace SistemaGestaoEscolar.Alunos.Dominio.ObjetosDeValor;

public class Genero : ValueObject
{
    public static readonly Genero Masculino = new(TipoGenero.Masculino);
    public static readonly Genero Feminino = new(TipoGenero.Feminino);
    public static readonly Genero NaoBinario = new(TipoGenero.NaoBinario);
    public static readonly Genero NaoInformado = new(TipoGenero.NaoInformado);
    
    public TipoGenero Tipo { get; private set; }
    
    private Genero(TipoGenero tipo)
    {
        Tipo = tipo;
    }
    
    public static Genero Criar(TipoGenero? tipo = null)
    {
        return tipo switch
        {
            TipoGenero.Masculino => Masculino,
            TipoGenero.Feminino => Feminino,
            TipoGenero.NaoBinario => NaoBinario,
            _ => NaoInformado // Padrão quando não informado
        };
    }
    
    public static Genero Criar(int? tipoInt = null)
    {
        if (!tipoInt.HasValue)
            return NaoInformado;
            
        if (!Enum.IsDefined(typeof(TipoGenero), tipoInt.Value))
            return NaoInformado;
            
        return Criar((TipoGenero)tipoInt.Value);
    }
    
    public override string ToString()
    {
        return Tipo switch
        {
            TipoGenero.Masculino => "Masculino",
            TipoGenero.Feminino => "Feminino",
            TipoGenero.NaoBinario => "Não Binário",
            _ => "Não Informado"
        };
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Tipo;
    }
}

public enum TipoGenero
{
    NaoInformado = 0,
    Masculino = 1,
    Feminino = 2,
    NaoBinario = 3
}