using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands;

public class ReativarDisciplinaCommand : IRequest
{
    public Guid Id { get; set; }
}