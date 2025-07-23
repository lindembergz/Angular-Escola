using SistemaGestaoEscolar.Shared.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace SistemaGestaoEscolar.Alunos.Dominio.ObjetosDeValor;

public class CPF : ValueObject
{
    public string Numero { get; private set; } = string.Empty;
    public string NumeroFormatado => FormatarCpf(Numero);

    private CPF() { } // Para EF Core

    public CPF(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("CPF não pode ser vazio", nameof(numero));

        var cpfLimpo = LimparCpf(numero);
        
        if (!ValidarCpf(cpfLimpo))
            throw new ArgumentException("CPF inválido", nameof(numero));

        Numero = cpfLimpo;
    }

    private static string LimparCpf(string cpf)
    {
        return Regex.Replace(cpf, @"[^\d]", "");
    }

    private static bool ValidarCpf(string cpf)
    {
        if (cpf.Length != 11)
            return false;

        // Verificar se todos os dígitos são iguais
        if (cpf.All(c => c == cpf[0]))
            return false;

        // Calcular primeiro dígito verificador
        int soma = 0;
        for (int i = 0; i < 9; i++)
        {
            soma += int.Parse(cpf[i].ToString()) * (10 - i);
        }
        
        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;
        
        if (int.Parse(cpf[9].ToString()) != digito1)
            return false;

        // Calcular segundo dígito verificador
        soma = 0;
        for (int i = 0; i < 10; i++)
        {
            soma += int.Parse(cpf[i].ToString()) * (11 - i);
        }
        
        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;
        
        return int.Parse(cpf[10].ToString()) == digito2;
    }

    private static string FormatarCpf(string cpf)
    {
        if (cpf.Length != 11)
            return cpf;

        return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Numero;
    }

    public override string ToString() => NumeroFormatado;

    public static implicit operator string(CPF cpf) => cpf?.Numero ?? string.Empty;
    public static implicit operator CPF(string numero) => new(numero);
}