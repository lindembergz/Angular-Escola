namespace SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;

public class RedeEscolarRespostaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CnpjMantenedora { get; set; } = string.Empty;
    public EnderecoDto EnderecoSede { get; set; } = new();
    public DateTime DataCriacao { get; set; }
    public bool Ativa { get; set; }
    public int TotalEscolas { get; set; }
    public int EscolasAtivas { get; set; }
    public int EscolasInativas { get; set; }
    public List<EscolaRespostaDto> Escolas { get; set; } = new();
}