
using MediatR;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands
{
    public class CriarTurmaCommand : IRequest<Guid>
    {
        public string Nome { get; set; }
        public TipoSerie TipoSerie { get; set; }
        public int AnoSerie { get; set; }
        public TipoTurno TipoTurno { get; set; }
        public TimeOnly HoraInicioTurno { get; set; }
        public TimeOnly HoraFimTurno { get; set; }
        public int CapacidadeMaxima { get; set; }
        public int AnoLetivo { get; set; }
        public Guid UnidadeEscolarId { get; set; }
    }
}
