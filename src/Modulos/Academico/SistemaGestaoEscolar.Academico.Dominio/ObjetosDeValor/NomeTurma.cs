using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;

public class NomeTurma : ValueObject
{
    public string Valor { get; private set; }

    private NomeTurma(string valor)
    {
        Valor = valor;
    }

    public static NomeTurma Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Nome da turma não pode ser vazio");

        if (valor.Length > 50)
            throw new ArgumentException("Nome da turma não pode ter mais de 50 caracteres");

        return new NomeTurma(valor.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor;
    }

    public override string ToString() => Valor;
}