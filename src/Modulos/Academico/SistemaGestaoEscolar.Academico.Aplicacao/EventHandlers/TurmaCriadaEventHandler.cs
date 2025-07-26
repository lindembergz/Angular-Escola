
using MediatR;
using SistemaGestaoEscolar.Academico.Dominio.Eventos;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaGestaoEscolar.Academico.Aplicacao.EventHandlers
{
    public class TurmaCriadaEventHandler : INotificationHandler<TurmaCriadaEvento>
    {
        // Injetar serviços necessários, como um barramento de eventos para comunicação entre módulos
        public TurmaCriadaEventHandler()
        {
            
        }

        public Task Handle(TurmaCriadaEvento notification, CancellationToken cancellationToken)
        {
            // Lógica para quando uma turma é criada.
            // Ex: Publicar um evento de integração para outros módulos saberem da nova turma.
            // _eventBus.Publish(new TurmaCriadaIntegrationEvent(notification.TurmaId));
            
            return Task.CompletedTask;
        }
    }
}
