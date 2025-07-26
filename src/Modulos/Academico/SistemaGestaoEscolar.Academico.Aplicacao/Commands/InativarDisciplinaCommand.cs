using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands;

public class InativarDisciplinaCommand : IRequest
{
    public Guid Id { get; set; }
}