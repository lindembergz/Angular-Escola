
using MediatR;
using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;
using SistemaGestaoEscolar.Academico.Dominio.Servicos;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaGestaoEscolar.Academico.Aplicacao.CommandHandlers
{
    public class CriarHorarioCommandHandler : IRequestHandler<Commands.CriarHorarioCommand, Guid>
    {
        private readonly IRepositorioHorario _repositorioHorario;
        private readonly IServicosDominioHorario _servicosDominioHorario;

        public CriarHorarioCommandHandler(IRepositorioHorario repositorioHorario, IServicosDominioHorario servicosDominioHorario)
        {
            _repositorioHorario = repositorioHorario;
            _servicosDominioHorario = servicosDominioHorario;
        }

        public async Task<Guid> Handle(Commands.CriarHorarioCommand request, CancellationToken cancellationToken)
        {
            var slotTempo = Dominio.ObjetosDeValor.SlotTempo.Criar(request.HoraInicio, request.HoraFim, request.DiaDaSemana);

            var horario =  Horario.Criar(
                request.TurmaId,
                request.DisciplinaId,
                request.ProfessorId,
                slotTempo,
                request.AnoLetivo,
                request.Semestre,
                request.Sala
            );

            await _repositorioHorario.AdicionarAsync(horario);

            return horario.Id;
        }
    }
}
