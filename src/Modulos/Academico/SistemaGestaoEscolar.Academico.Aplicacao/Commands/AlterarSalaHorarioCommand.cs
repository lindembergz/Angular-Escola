using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands;

public class AlterarSalaHorarioCommand : IRequest
{
    public Guid HorarioId { get; set; }
    public string? NovaSala { get; set; }
}