
using MediatR;
using SistemaGestaoEscolar.Academico.Dominio.Eventos;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaGestaoEscolar.Academico.Aplicacao.EventHandlers
{
    public class DisciplinaCriadaEventHandler : INotificationHandler<DisciplinaCriadaEvento>
    {
        public Task Handle(DisciplinaCriadaEvento notification, CancellationToken cancellationToken)
        {
            // Lógica para quando uma disciplina é criada.
            return Task.CompletedTask;
        }
    }
}
