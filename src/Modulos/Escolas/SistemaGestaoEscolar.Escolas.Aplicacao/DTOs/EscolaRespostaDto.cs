namespace SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;

public class EscolaRespostaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public EnderecoDto Endereco { get; set; } = new();
    public string Tipo { get; set; } = string.Empty;
    public Guid? RedeEscolarId { get; set; }
    public string? NomeRedeEscolar { get; set; }
    public DateTime DataCriacao { get; set; }
    public bool Ativa { get; set; }
    public List<UnidadeEscolarRespostaDto> Unidades { get; set; } = new();
}