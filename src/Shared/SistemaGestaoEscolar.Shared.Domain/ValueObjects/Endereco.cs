using System.Text.RegularExpressions;

namespace SistemaGestaoEscolar.Shared.Domain.ValueObjects;

public class Endereco : ValueObject
{
    public string Logradouro { get; private set; } = string.Empty;
    public string Numero { get; private set; } = string.Empty;
    public string? Complemento { get; private set; }
    public string Bairro { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;
    public string Estado { get; private set; } = string.Empty;
    public string Cep { get; private set; } = string.Empty;
    public string Pais { get; private set; } = string.Empty;

    private Endereco() { } // Para EF Core

    public Endereco(
        string logradouro,
        string numero,
        string bairro,
        string cidade,
        string estado,
        string cep,
        string? complemento = null,
        string pais = "Brasil")
    {
        if (string.IsNullOrWhiteSpace(logradouro))
            throw new ArgumentException("Logradouro é obrigatório", nameof(logradouro));

        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Número é obrigatório", nameof(numero));

        if (string.IsNullOrWhiteSpace(bairro))
            throw new ArgumentException("Bairro é obrigatório", nameof(bairro));

        if (string.IsNullOrWhiteSpace(cidade))
            throw new ArgumentException("Cidade é obrigatória", nameof(cidade));

        if (string.IsNullOrWhiteSpace(estado))
            throw new ArgumentException("Estado é obrigatório", nameof(estado));

        if (string.IsNullOrWhiteSpace(cep))
            throw new ArgumentException("CEP é obrigatório", nameof(cep));

        var cepLimpo = LimparCep(cep);
        if (!ValidarCep(cepLimpo))
            throw new ArgumentException("CEP inválido", nameof(cep));

        if (!ValidarEstado(estado))
            throw new ArgumentException("Estado inválido", nameof(estado));

        Logradouro = logradouro.Trim();
        Numero = numero.Trim();
        Complemento = string.IsNullOrWhiteSpace(complemento) ? null : complemento.Trim();
        Bairro = bairro.Trim();
        Cidade = cidade.Trim();
        Estado = estado.Trim().ToUpper();
        Cep = cepLimpo;
        Pais = string.IsNullOrWhiteSpace(pais) ? "Brasil" : pais.Trim();
    }

    private static string LimparCep(string cep)
    {
        return Regex.Replace(cep, @"[^\d]", "");
    }

    private static bool ValidarCep(string cep)
    {
        return cep.Length == 8 && cep.All(char.IsDigit);
    }

    private static bool ValidarEstado(string estado)
    {
        var estadosValidos = new[]
        {
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA",
            "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN",
            "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        };

        return estadosValidos.Contains(estado.ToUpper());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Logradouro.ToUpperInvariant();
        yield return Numero.ToUpperInvariant();
        yield return Complemento?.ToUpperInvariant() ?? "";
        yield return Bairro.ToUpperInvariant();
        yield return Cidade.ToUpperInvariant();
        yield return Estado;
        yield return Cep;
        yield return Pais.ToUpperInvariant();
    }

    public override string ToString()
    {
        var endereco = $"{Logradouro}, {Numero}";
        
        if (!string.IsNullOrWhiteSpace(Complemento))
            endereco += $", {Complemento}";
            
        endereco += $", {Bairro}, {Cidade} - {Estado}, {CepFormatado}";
        
        if (Pais != "Brasil")
            endereco += $", {Pais}";
            
        return endereco;
    }

    public string CepFormatado => Cep.Length == 8 ? $"{Cep.Substring(0, 5)}-{Cep.Substring(5, 3)}" : Cep;

    public string EnderecoCompleto => ToString();

    public string EnderecoResumido => $"{Logradouro}, {Numero} - {Bairro}, {Cidade}/{Estado}";

    public bool EhDoMesmoEstado(Endereco outroEndereco)
    {
        return outroEndereco != null && Estado == outroEndereco.Estado;
    }

    public bool EhDaMesmaCidade(Endereco outroEndereco)
    {
        return outroEndereco != null && 
               Estado == outroEndereco.Estado && 
               Cidade.Equals(outroEndereco.Cidade, StringComparison.OrdinalIgnoreCase);
    }

    public bool EhDoMesmoBairro(Endereco outroEndereco)
    {
        return EhDaMesmaCidade(outroEndereco) && 
               Bairro.Equals(outroEndereco.Bairro, StringComparison.OrdinalIgnoreCase);
    }

    public string ObterRegiao()
    {
        var regioes = new Dictionary<string, string[]>
        {
            ["Norte"] = new[] { "AC", "AP", "AM", "PA", "RO", "RR", "TO" },
            ["Nordeste"] = new[] { "AL", "BA", "CE", "MA", "PB", "PE", "PI", "RN", "SE" },
            ["Centro-Oeste"] = new[] { "DF", "GO", "MT", "MS" },
            ["Sudeste"] = new[] { "ES", "MG", "RJ", "SP" },
            ["Sul"] = new[] { "PR", "RS", "SC" }
        };

        return regioes.FirstOrDefault(r => r.Value.Contains(Estado)).Key ?? "Desconhecida";
    }

    public bool EhCapital()
    {
        var capitais = new Dictionary<string, string>
        {
            ["AC"] = "Rio Branco", ["AL"] = "Maceió", ["AP"] = "Macapá", ["AM"] = "Manaus",
            ["BA"] = "Salvador", ["CE"] = "Fortaleza", ["DF"] = "Brasília", ["ES"] = "Vitória",
            ["GO"] = "Goiânia", ["MA"] = "São Luís", ["MT"] = "Cuiabá", ["MS"] = "Campo Grande",
            ["MG"] = "Belo Horizonte", ["PA"] = "Belém", ["PB"] = "João Pessoa", ["PR"] = "Curitiba",
            ["PE"] = "Recife", ["PI"] = "Teresina", ["RJ"] = "Rio de Janeiro", ["RN"] = "Natal",
            ["RS"] = "Porto Alegre", ["RO"] = "Porto Velho", ["RR"] = "Boa Vista", ["SC"] = "Florianópolis",
            ["SP"] = "São Paulo", ["SE"] = "Aracaju", ["TO"] = "Palmas"
        };

        return capitais.TryGetValue(Estado, out var capital) && 
               Cidade.Equals(capital, StringComparison.OrdinalIgnoreCase);
    }
}