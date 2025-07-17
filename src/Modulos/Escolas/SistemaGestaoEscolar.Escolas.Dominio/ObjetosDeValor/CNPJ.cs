using SistemaGestaoEscolar.Shared.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;

public class CNPJ : ValueObject
{
    public string Numero { get; private set; } = string.Empty;
    public string NumeroFormatado => FormatarCnpj(Numero);

    private CNPJ() { } // Para EF Core

    public CNPJ(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("CNPJ não pode ser vazio", nameof(numero));

        var cnpjLimpo = LimparCnpj(numero);
        
        if (!ValidarCnpj(cnpjLimpo))
            throw new ArgumentException("CNPJ inválido", nameof(numero));

        Numero = cnpjLimpo;
    }

    private static string LimparCnpj(string cnpj)
    {
        return Regex.Replace(cnpj, @"[^\d]", "");
    }

    private static bool ValidarCnpj(string cnpj)
    {
        if (cnpj.Length != 14)
            return false;

        // Verificar se todos os dígitos são iguais
        if (cnpj.All(c => c == cnpj[0]))
            return false;

        // Calcular primeiro dígito verificador
        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int soma = 0;
        
        for (int i = 0; i < 12; i++)
        {
            soma += int.Parse(cnpj[i].ToString()) * multiplicador1[i];
        }
        
        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;
        
        if (int.Parse(cnpj[12].ToString()) != digito1)
            return false;

        // Calcular segundo dígito verificador
        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        soma = 0;
        
        for (int i = 0; i < 13; i++)
        {
            soma += int.Parse(cnpj[i].ToString()) * multiplicador2[i];
        }
        
        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;
        
        return int.Parse(cnpj[13].ToString()) == digito2;
    }

    private static string FormatarCnpj(string cnpj)
    {
        if (cnpj.Length != 14)
            return cnpj;

        return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Numero;
    }

    public override string ToString() => NumeroFormatado;

    public static implicit operator string(CNPJ cnpj) => cnpj?.Numero ?? string.Empty;
    public static implicit operator CNPJ(string numero) => new(numero);

    public string ObterRaizCnpj()
    {
        return Numero.Substring(0, 8); // Primeiros 8 dígitos identificam a empresa
    }

    public string ObterFilial()
    {
        return Numero.Substring(8, 4); // Dígitos 9-12 identificam a filial
    }

    public string ObterDigitosVerificadores()
    {
        return Numero.Substring(12, 2); // Últimos 2 dígitos são verificadores
    }

    public bool EhMatriz()
    {
        return ObterFilial() == "0001";
    }

    public bool EhFilial()
    {
        return !EhMatriz();
    }

    public bool PertenceAoMesmoGrupo(CNPJ outroCnpj)
    {
        if (outroCnpj == null)
            return false;

        return ObterRaizCnpj() == outroCnpj.ObterRaizCnpj();
    }
}