using MediatR;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands;

public class AdicionarPreRequisitoCommand : IRequest
{
    public Guid DisciplinaId { get; set; }
    public Guid PreRequisitoId { get; set; }
}