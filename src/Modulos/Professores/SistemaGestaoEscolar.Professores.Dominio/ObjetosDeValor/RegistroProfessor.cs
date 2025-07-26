using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Professores.Dominio.ObjetosDeValor;

public class RegistroProfessor : ValueObject
{
    public string Numero { get; private set; } = string.Empty;
    public string NumeroFormatado => FormatarNumero();

    private RegistroProfessor() { } // Para EF Core

    public RegistroProfessor(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Número de registro do professor não pode ser vazio", nameof(numero));

        // Remove caracteres não numéricos
        var numeroLimpo = new string(numero.Where(char.IsDigit).ToArray());

        if (numeroLimpo.Length < 4)
            throw new ArgumentException("Número de registro deve ter pelo menos 4 dígitos", nameof(numero));

        if (numeroLimpo.Length > 20)
            throw new ArgumentException("Número de registro não pode ter mais de 20 dígitos", nameof(numero));

        Numero = numeroLimpo;
    }

    private string FormatarNumero()
    {
        if (Numero.Length <= 6)
            return Numero;

        // Formato padrão: XXXX.XXXX.XX
        if (Numero.Length <= 10)
        {
            return $"{Numero.Substring(0, 4)}.{Numero.Substring(4, Math.Min(4, Numero.Length - 4))}" +
                   (Numero.Length > 8 ? $".{Numero.Substring(8)}" : "");
        }

        // Para números maiores, agrupa de 4 em 4
        var grupos = new List<string>();
        for (int i = 0; i < Numero.Length; i += 4)
        {
            grupos.Add(Numero.Substring(i, Math.Min(4, Numero.Length - i)));
        }
        return string.Join(".", grupos);
    }

    public bool EhValido()
    {
        return !string.IsNullOrEmpty(Numero) && Numero.Length >= 4;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Numero;
    }

    public override string ToString() => NumeroFormatado;

    public static implicit operator string(RegistroProfessor registro) => registro?.Numero ?? string.Empty;
    public static implicit operator RegistroProfessor(string numero) => new(numero);
}