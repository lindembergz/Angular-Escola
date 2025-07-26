
using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;
using System.Collections.Generic;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries
{
    public class ObterDisciplinasQuery : IRequest<IEnumerable<DisciplinaReadDto>>
    {
        // Pode ter filtros no futuro, como por série ou nível de ensino
        public Guid EscolaId { get; set; }
        public bool Ativa { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
