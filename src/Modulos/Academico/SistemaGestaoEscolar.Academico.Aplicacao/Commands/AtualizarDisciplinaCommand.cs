using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands;

public class AtualizarDisciplinaCommand : IRequest
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int CargaHoraria { get; set; }
    public bool Obrigatoria { get; set; }
    public string? Descricao { get; set; }
}