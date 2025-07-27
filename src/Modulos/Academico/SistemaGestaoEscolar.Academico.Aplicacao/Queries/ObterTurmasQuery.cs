
using MediatR;
using SistemaGestaoEscolar.Academico.Aplicacao.DTOs;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Queries
{
    public class ObterTurmasQuery : IRequest<ObterTurmasResponse>
    {
        public Guid? EscolaId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? AnoLetivo { get; set; }
        public string? Serie { get; set; }
        public string? Turno { get; set; }
        public bool? Ativa { get; set; }
    }

    public class ObterTurmasResponse
    {
        public IEnumerable<TurmaResumoReadDto> Turmas { get; set; } = new List<TurmaResumoReadDto>();
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
