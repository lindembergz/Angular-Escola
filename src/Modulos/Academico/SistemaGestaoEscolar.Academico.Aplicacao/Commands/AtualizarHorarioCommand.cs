using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands;

public class AtualizarHorarioCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid ProfessorId { get; set; }
    public string? Sala { get; set; }
}