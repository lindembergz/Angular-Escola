using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands;

public class ReativarHorarioCommand : IRequest
{
    public Guid Id { get; set; }
}