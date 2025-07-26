using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands;

public class ReativarTurmaCommand : IRequest
{
    public Guid Id { get; set; }
}