
using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;
using System.Collections.Generic;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries
{
    public class ObterTurmasQuery : IRequest<IEnumerable<TurmaResumoReadDto>>
    {
        public Guid UnidadeEscolarId { get; set; }
    }
}
