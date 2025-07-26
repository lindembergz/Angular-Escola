using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands;

public class CancelarHorarioCommand : IRequest
{
    public Guid Id { get; set; }
}