namespace SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;

public class AdicionarUnidadeEscolarDto
{
    public Guid EscolaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public EnderecoDto Endereco { get; set; } = new();
    public string Tipo { get; set; } = string.Empty; // "Infantil", "Fundamental", "Medio", etc.
    public int CapacidadeMaximaAlunos { get; set; }
    
    // Configurações específicas para ensino infantil
    public int? IdadeMinima { get; set; }
    public int? IdadeMaxima { get; set; }
    public bool TemBerçario { get; set; }
    public bool TemPreEscola { get; set; }
    
    // Séries atendidas (para fundamental e médio)
    public List<string> SeriesAtendidas { get; set; } = new();
}