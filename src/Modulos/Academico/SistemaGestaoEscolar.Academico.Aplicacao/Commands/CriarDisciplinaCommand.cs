
using MediatR;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;

namespace SistemaGestaoEscolar.Academico.Aplicacao.Commands
{
    public class CriarDisciplinaCommand : IRequest<Guid>
    {
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public int CargaHoraria { get; set; }
        public TipoSerie TipoSerie { get; set; }
        public int AnoSerie { get; set; }
        public bool Obrigatoria { get; set; }
        public Guid EscolaId { get; set; }
        public string? Descricao { get; set; }
    }
}
