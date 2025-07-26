
using MediatR;
using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaGestaoEscolar.Academico.Aplicacao.CommandHandlers
{
    public class CriarTurmaCommandHandler : IRequestHandler<Commands.CriarTurmaCommand, Guid>
    {
        private readonly IRepositorioTurma _repositorioTurma;

        public CriarTurmaCommandHandler(IRepositorioTurma repositorioTurma)
        {
            _repositorioTurma = repositorioTurma;
        }

        public async Task<Guid> Handle(Commands.CriarTurmaCommand request, CancellationToken cancellationToken)
        {
            var nomeTurma = NomeTurma.Criar(request.Nome);
            var serie = Serie.Criar(request.TipoSerie,request.AnoSerie);
            var turno = Turno.Criar(request.TipoTurno, request.HoraInicioTurno, request.HoraFimTurno);

            var turma = Turma.Criar(
                nomeTurma,
                serie,
                turno,
                request.CapacidadeMaxima,
                DateTime.Now.Year, // AnoLetivo
                request.UnidadeEscolarId
            );

            await _repositorioTurma.AdicionarAsync(turma);

            return turma.Id;
        }
    }
}
