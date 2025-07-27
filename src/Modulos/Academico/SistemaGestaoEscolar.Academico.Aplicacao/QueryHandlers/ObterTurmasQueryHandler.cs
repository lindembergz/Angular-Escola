
using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;
using SistemaGestaoEscolar.Academico.Aplicacao.Interfaces;
using SistemaGestaoEscolar.Academico.Aplicacao.Queries;

namespace SistemaGestaoEscolar.Academico.Aplicacao.QueryHandlers
{
    public class ObterTurmasQueryHandler : IRequestHandler<ObterTurmasQuery, ObterTurmasResponse>
    {
        private readonly ITurmaQueryService _turmaQueryService;

        public ObterTurmasQueryHandler(ITurmaQueryService turmaQueryService)
        {
            _turmaQueryService = turmaQueryService;
        }

        public async Task<ObterTurmasResponse> Handle(ObterTurmasQuery request, CancellationToken cancellationToken)
        {
            var (turmas, totalItems) = await _turmaQueryService.ObterTurmasComFiltrosAsync(
                escolaId: request.EscolaId,
                anoLetivo: request.AnoLetivo,
                serie: request.Serie,
                turno: request.Turno,
                ativa: request.Ativa,
                page: request.Page,
                pageSize: request.PageSize
            );

            var totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

            return new ObterTurmasResponse
            {
                Turmas = turmas,
                TotalItems = totalItems,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }


    }
}
