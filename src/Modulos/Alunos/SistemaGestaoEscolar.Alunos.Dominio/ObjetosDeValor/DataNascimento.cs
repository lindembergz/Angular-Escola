using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Alunos.Dominio.ObjetosDeValor;

public class DataNascimento : ValueObject
{
    public DateTime Valor { get; private set; }
    public int Idade => CalcularIdade();
    public string IdadeFormatada => $"{Idade} anos";
    public bool EhMaiorIdade => Idade >= 18;
    public bool EhCrianca => Idade < 12;
    public bool EhAdolescente => Idade >= 12 && Idade < 18;

    private DataNascimento() { } // Para EF Core

    public DataNascimento(DateTime valor)
    {
        if (valor == default)
            throw new ArgumentException("Data de nascimento não pode ser vazia", nameof(valor));

        if (valor > DateTime.Today)
            throw new ArgumentException("Data de nascimento não pode ser futura", nameof(valor));

        if (valor < DateTime.Today.AddYears(-120))
            throw new ArgumentException("Data de nascimento não pode ser anterior a 120 anos", nameof(valor));

        // Normalizar para apenas a data (sem horário)
        Valor = valor.Date;
    }

    public DataNascimento(int ano, int mes, int dia) : this(new DateTime(ano, mes, dia))
    {
    }

    private int CalcularIdade()
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - Valor.Year;

        // Verificar se ainda não fez aniversário este ano
        if (hoje.Month < Valor.Month || (hoje.Month == Valor.Month && hoje.Day < Valor.Day))
        {
            idade--;
        }

        return idade;
    }

    public bool FezAniversarioEsteAno()
    {
        var hoje = DateTime.Today;
        var aniversarioEsteAno = new DateTime(hoje.Year, Valor.Month, Valor.Day);
        return aniversarioEsteAno <= hoje;
    }

    public DateTime ProximoAniversario()
    {
        var hoje = DateTime.Today;
        var aniversarioEsteAno = new DateTime(hoje.Year, Valor.Month, Valor.Day);
        
        if (aniversarioEsteAno >= hoje)
            return aniversarioEsteAno;
        
        return aniversarioEsteAno.AddYears(1);
    }

    public int DiasParaProximoAniversario()
    {
        return (ProximoAniversario() - DateTime.Today).Days;
    }

    public string ObterSigno()
    {
        return (Valor.Month, Valor.Day) switch
        {
            (3, >= 21) or (4, <= 19) => "Áries",
            (4, >= 20) or (5, <= 20) => "Touro",
            (5, >= 21) or (6, <= 20) => "Gêmeos",
            (6, >= 21) or (7, <= 22) => "Câncer",
            (7, >= 23) or (8, <= 22) => "Leão",
            (8, >= 23) or (9, <= 22) => "Virgem",
            (9, >= 23) or (10, <= 22) => "Libra",
            (10, >= 23) or (11, <= 21) => "Escorpião",
            (11, >= 22) or (12, <= 21) => "Sagitário",
            (12, >= 22) or (1, <= 19) => "Capricórnio",
            (1, >= 20) or (2, <= 18) => "Aquário",
            _ => "Peixes"
        };
    }

    public bool EhIdadeEscolar()
    {
        return Idade >= 4 && Idade <= 17;
    }

    public string ObterFaixaEtariaEscolar()
    {
        return Idade switch
        {
            >= 4 and <= 5 => "Educação Infantil",
            >= 6 and <= 10 => "Ensino Fundamental I",
            >= 11 and <= 14 => "Ensino Fundamental II",
            >= 15 and <= 17 => "Ensino Médio",
            _ => "Fora da faixa escolar regular"
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor;
    }

    public override string ToString() => Valor.ToString("dd/MM/yyyy");

    public static implicit operator DateTime(DataNascimento data) => data?.Valor ?? default;
    public static implicit operator DataNascimento(DateTime valor) => new(valor);
}