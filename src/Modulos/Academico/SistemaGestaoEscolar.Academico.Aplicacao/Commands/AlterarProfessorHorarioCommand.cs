using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands;

public class AlterarProfessorHorarioCommand : IRequest
{
    public Guid HorarioId { get; set; }
    public Guid NovoProfessorId { get; set; }
}