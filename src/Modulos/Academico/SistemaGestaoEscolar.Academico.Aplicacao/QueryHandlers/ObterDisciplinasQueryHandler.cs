
using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;
using SistemaGestaoEscolar.Academico.Aplicacao.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Academico.Aplicacao.QueryHandlers
{
    public class ObterDisciplinasQueryHandler : IRequestHandler<ObterDisciplinasQuery, IEnumerable<DisciplinaReadDto>>
    {
        private readonly IRepositorioDisciplina _repositorioDisciplina;

        public ObterDisciplinasQueryHandler(IRepositorioDisciplina repositorioDisciplina)
        {
            _repositorioDisciplina = repositorioDisciplina;
        }

        public async Task<IEnumerable<DisciplinaReadDto>> Handle(ObterDisciplinasQuery request, CancellationToken cancellationToken)
        {
            var disciplinas = await _repositorioDisciplina.ObterTodosAsync();
            return disciplinas.Select(d => new DisciplinaReadDto(
                d.Id,
                d.Nome,
                d.Codigo,
                d.CargaHoraria,
                d.Serie.Descricao,
                d.Obrigatoria,
                d.Descricao,
                d.EscolaId,
                d.Ativa,
                d.PreRequisitos.ToList(),
                d.CreatedAt
            ));
        }
    }
}
