namespace SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;

public class CriarRedeEscolarDto
{
    public string Nome { get; set; } = string.Empty;
    public string CnpjMantenedora { get; set; } = string.Empty;
    public EnderecoDto EnderecoSede { get; set; } = new();
}