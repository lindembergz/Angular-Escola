using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands;

public class InativarTurmaCommand : IRequest
{
    public Guid Id { get; set; }
}