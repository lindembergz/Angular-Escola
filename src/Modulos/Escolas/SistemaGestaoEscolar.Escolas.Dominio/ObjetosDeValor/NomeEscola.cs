using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;

public class NomeEscola : ValueObject
{
    public string Valor { get; private set; } = string.Empty;

    private NomeEscola() { } // Para EF Core

    public NomeEscola(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Nome da escola não pode ser vazio", nameof(valor));

        if (valor.Length < 3)
            throw new ArgumentException("Nome da escola deve ter pelo menos 3 caracteres", nameof(valor));

        if (valor.Length > 200)
            throw new ArgumentException("Nome da escola não pode ter mais de 200 caracteres", nameof(valor));

        // Validar caracteres especiais
        if (valor.Any(c => char.IsControl(c) || c == '<' || c == '>' || c == '"' || c == '\''))
            throw new ArgumentException("Nome da escola contém caracteres inválidos", nameof(valor));

        Valor = valor.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor.ToUpperInvariant(); // Case-insensitive comparison
    }

    public override string ToString() => Valor;

    public static implicit operator string(NomeEscola nomeEscola) => nomeEscola?.Valor ?? string.Empty;
    public static implicit operator NomeEscola(string valor) => new(valor);

    public bool ContemPalavra(string palavra)
    {
        if (string.IsNullOrWhiteSpace(palavra))
            return false;

        return Valor.Contains(palavra, StringComparison.OrdinalIgnoreCase);
    }

    public string ObterIniciais()
    {
        var palavras = Valor.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join("", palavras.Select(p => char.ToUpper(p[0])));
    }

    public string ObterNomeAbreviado(int tamanhoMaximo = 50)
    {
        if (Valor.Length <= tamanhoMaximo)
            return Valor;

        var palavras = Valor.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var resultado = palavras[0];

        for (int i = 1; i < palavras.Length && resultado.Length + palavras[i].Length + 1 <= tamanhoMaximo; i++)
        {
            resultado += " " + palavras[i];
        }

        return resultado + (resultado.Length < Valor.Length ? "..." : "");
    }
}