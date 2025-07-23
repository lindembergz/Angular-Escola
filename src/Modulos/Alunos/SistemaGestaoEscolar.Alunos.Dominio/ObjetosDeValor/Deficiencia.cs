using SistemaGestaoEscolar.Shared.Domain.ValueObjects;
using SistemaGestaoEscolar.Shared.Domain.Exceptions;

namespace SistemaGestaoEscolar.Alunos.Dominio.ObjetosDeValor;

public class Deficiencia : ValueObject
{
    public TipoDeficiencia? Tipo { get; private set; }
    public string Descricao { get; private set; }
    public bool PossuiDeficiencia => Tipo.HasValue;
    
    private Deficiencia(TipoDeficiencia? tipo, string descricao)
    {
        Tipo = tipo;
        Descricao = descricao ?? string.Empty;
    }
    
    public static Deficiencia Nenhuma()
    {
        return new Deficiencia(null, string.Empty);
    }
    
    public static Deficiencia Criar(TipoDeficiencia tipo, string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new DomainException("Descrição da deficiência é obrigatória quando tipo é informado");
            
        return new Deficiencia(tipo, descricao.Trim());
    }
    
    public static Deficiencia Criar(int? tipoInt, string descricao)
    {
        if (!tipoInt.HasValue || string.IsNullOrWhiteSpace(descricao))
            return Nenhuma();
            
        if (!Enum.IsDefined(typeof(TipoDeficiencia), tipoInt.Value))
            throw new DomainException($"Tipo de deficiência inválido: {tipoInt.Value}");
            
        return Criar((TipoDeficiencia)tipoInt.Value, descricao);
    }
    
    public override string ToString()
    {
        if (!PossuiDeficiencia)
            return "Nenhuma deficiência";
            
        var tipoDescricao = Tipo switch
        {
            TipoDeficiencia.Visual => "Visual",
            TipoDeficiencia.Auditiva => "Auditiva",
            TipoDeficiencia.Fisica => "Física",
            TipoDeficiencia.Intelectual => "Intelectual",
            TipoDeficiencia.Multipla => "Múltipla",
            TipoDeficiencia.Autismo => "Autismo",
            TipoDeficiencia.Surdocegueira => "Surdocegueira",
            _ => "Não especificada"
        };
        
        return $"{tipoDescricao}: {Descricao}";
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Tipo ?? (object)"None";
        yield return Descricao;
    }
}

public enum TipoDeficiencia
{
    Visual = 1,
    Auditiva = 2,
    Fisica = 3,
    Intelectual = 4,
    Multipla = 5,
    Autismo = 6,
    Surdocegueira = 7
}