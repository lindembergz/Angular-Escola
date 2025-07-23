using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Alunos.Dominio.ObjetosDeValor;

public class NomeAluno : ValueObject
{
    public string Valor { get; private set; } = string.Empty;
    public string PrimeiroNome => ObterPrimeiroNome();
    public string UltimoNome => ObterUltimoNome();
    public string NomeCompleto => Valor;
    public string Iniciais => ObterIniciais();

    private NomeAluno() { } // Para EF Core

    public NomeAluno(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Nome do aluno não pode ser vazio", nameof(valor));

        if (valor.Length < 2)
            throw new ArgumentException("Nome do aluno deve ter pelo menos 2 caracteres", nameof(valor));

        if (valor.Length > 100)
            throw new ArgumentException("Nome do aluno não pode ter mais de 100 caracteres", nameof(valor));

        // Validar se contém apenas letras, espaços e acentos
        if (!System.Text.RegularExpressions.Regex.IsMatch(valor, @"^[a-zA-ZÀ-ÿ\s]+$"))
            throw new ArgumentException("Nome do aluno deve conter apenas letras e espaços", nameof(valor));

        // Normalizar o nome (primeira letra maiúscula, demais minúsculas)
        Valor = NormalizarNome(valor.Trim());
    }

    private static string NormalizarNome(string nome)
    {
        var palavras = nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var palavrasNormalizadas = new List<string>();

        foreach (var palavra in palavras)
        {
            if (palavra.Length == 1)
            {
                palavrasNormalizadas.Add(palavra.ToUpper());
            }
            else if (EhPreposicao(palavra.ToLower()))
            {
                palavrasNormalizadas.Add(palavra.ToLower());
            }
            else
            {
                palavrasNormalizadas.Add(
                    char.ToUpper(palavra[0]) + palavra.Substring(1).ToLower());
            }
        }

        return string.Join(" ", palavrasNormalizadas);
    }

    private static bool EhPreposicao(string palavra)
    {
        var preposicoes = new[] { "de", "da", "do", "das", "dos", "e", "em", "na", "no", "para", "por" };
        return preposicoes.Contains(palavra);
    }

    private string ObterPrimeiroNome()
    {
        var partes = Valor.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return partes.Length > 0 ? partes[0] : string.Empty;
    }

    private string ObterUltimoNome()
    {
        var partes = Valor.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return partes.Length > 1 ? partes[^1] : string.Empty;
    }

    private string ObterIniciais()
    {
        var partes = Valor.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join("", partes.Select(p => p[0].ToString().ToUpper()));
    }

    public bool ContemNome(string busca)
    {
        if (string.IsNullOrWhiteSpace(busca))
            return false;

        return Valor.Contains(busca, StringComparison.OrdinalIgnoreCase);
    }

    public int QuantidadeNomes()
    {
        return Valor.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor.ToLowerInvariant();
    }

    public override string ToString() => Valor;

    public static implicit operator string(NomeAluno nome) => nome?.Valor ?? string.Empty;
    public static implicit operator NomeAluno(string valor) => new(valor);
}