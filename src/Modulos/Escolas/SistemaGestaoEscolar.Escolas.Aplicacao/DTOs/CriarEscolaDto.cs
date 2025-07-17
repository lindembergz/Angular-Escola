namespace SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;

public class CriarEscolaDto
{
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public EnderecoDto Endereco { get; set; } = new();
    public string Tipo { get; set; } = string.Empty; // "Infantil", "Fundamental", "Medio", "Tecnico", "Superior"
    public Guid? RedeEscolarId { get; set; }
}

public class EnderecoDto
{
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
}