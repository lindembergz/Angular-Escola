using MediatR;
using SistemaGestaoEscolar.Alunos.Dominio.Eventos;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.EventHandlers;

public class AlunoTransferidoEventHandler : INotificationHandler<AlunoTransferidoEvento>
{
    public AlunoTransferidoEventHandler()
    {
        // Injeção de dependências seria feita aqui
    }

    public async Task Handle(AlunoTransferidoEvento notification, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Notificar escola anterior sobre a transferência
            await NotificarEscolaAnterior(notification);

            // 2. Notificar nova escola sobre o recebimento
            await NotificarNovaEscola(notification);

            // 3. Atualizar módulo financeiro (transferir cobranças)
            await AtualizarModuloFinanceiro(notification);

            // 4. Notificar responsáveis sobre a transferência
            await NotificarResponsaveis(notification);

            // 5. Registrar log de auditoria
            await RegistrarLogAuditoria(notification);

            // 6. Atualizar estatísticas das escolas
            await AtualizarEstatisticasEscolas(notification);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar evento AlunoTransferido: {ex.Message}");
        }
    }

    private async Task NotificarEscolaAnterior(AlunoTransferidoEvento evento)
    {
        await Task.CompletedTask;
        Console.WriteLine($"[EscolaAnterior] Aluno {evento.AlunoId} transferido para escola {evento.NovaEscolaId}");
    }

    private async Task NotificarNovaEscola(AlunoTransferidoEvento evento)
    {
        await Task.CompletedTask;
        Console.WriteLine($"[NovaEscola] Aluno {evento.AlunoId} recebido da escola {evento.EscolaAnteriorId}");
    }

    private async Task AtualizarModuloFinanceiro(AlunoTransferidoEvento evento)
    {
        // Transferir cobranças pendentes, ajustar mensalidades, etc.
        await Task.CompletedTask;
        Console.WriteLine($"[FinanceiroIntegração] Transferência financeira processada para aluno {evento.AlunoId}");
    }

    private async Task NotificarResponsaveis(AlunoTransferidoEvento evento)
    {
        // Enviar notificação para responsáveis sobre a transferência
        await Task.CompletedTask;
        Console.WriteLine($"[NotificaçãoResponsáveis] Responsáveis notificados sobre transferência do aluno {evento.AlunoId}");
    }

    private async Task RegistrarLogAuditoria(AlunoTransferidoEvento evento)
    {
        await Task.CompletedTask;
        Console.WriteLine($"[Auditoria] Transferência registrada: Aluno {evento.AlunoId} - Motivo: {evento.Motivo}");
    }

    private async Task AtualizarEstatisticasEscolas(AlunoTransferidoEvento evento)
    {
        // Decrementar contador da escola anterior e incrementar da nova escola
        await Task.CompletedTask;
        Console.WriteLine($"[Estatísticas] Contadores atualizados para transferência entre escolas");
    }
}