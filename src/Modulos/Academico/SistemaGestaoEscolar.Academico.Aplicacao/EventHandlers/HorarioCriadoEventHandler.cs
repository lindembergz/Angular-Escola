
using MediatR;
using SistemaGestaoEscolar.Academico.Dominio.Eventos;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaGestaoEscolar.Academico.Aplicacao.EventHandlers
{
    public class HorarioCriadoEventHandler : INotificationHandler<HorarioCriadoEvento>
    {
        public Task Handle(HorarioCriadoEvento notification, CancellationToken cancellationToken)
        {
            // Lógica para quando um horário é criado.
            // Ex: Notificar professores e alunos.
            return Task.CompletedTask;
        }
    }
}
