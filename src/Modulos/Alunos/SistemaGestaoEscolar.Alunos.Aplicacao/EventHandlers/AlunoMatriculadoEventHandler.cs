using MediatR;
using SistemaGestaoEscolar.Alunos.Dominio.Eventos;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.EventHandlers;

public class AlunoMatriculadoEventHandler : INotificationHandler<AlunoMatriculadoEvento>
{
    public AlunoMatriculadoEventHandler()
    {
        // Injeção de dependências seria feita aqui
    }

    public async Task Handle(AlunoMatriculadoEvento notification, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Notificar módulo acadêmico sobre nova matrícula
            await NotificarModuloAcademico(notification);

            // 2. Configurar cobrança no módulo financeiro
            await ConfigurarCobrancaFinanceira(notification);

            // 3. Notificar responsáveis sobre a matrícula
            await NotificarResponsaveis(notification);

            // 4. Registrar no sistema de presença/frequência
            await RegistrarSistemaPresenca(notification);

            // 5. Registrar log de auditoria
            await RegistrarLogAuditoria(notification);

            // 6. Atualizar estatísticas da turma
            await AtualizarEstatisticasTurma(notification);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar evento AlunoMatriculado: {ex.Message}");
        }
    }

    private async Task NotificarModuloAcademico(AlunoMatriculadoEvento evento)
    {
        // Notificar módulo acadêmico para incluir aluno na turma
        await Task.CompletedTask;
        Console.WriteLine($"[AcadêmicoIntegração] Aluno {evento.AlunoId} adicionado à turma {evento.TurmaId}");
    }

    private async Task ConfigurarCobrancaFinanceira(AlunoMatriculadoEvento evento)
    {
        // Configurar mensalidades e taxas para o ano letivo
        await Task.CompletedTask;
        Console.WriteLine($"[FinanceiroIntegração] Cobrança configurada para matrícula {evento.MatriculaId} - Ano {evento.AnoLetivo}");
    }

    private async Task NotificarResponsaveis(AlunoMatriculadoEvento evento)
    {
        // Enviar confirmação de matrícula para responsáveis
        await Task.CompletedTask;
        Console.WriteLine($"[NotificaçãoResponsáveis] Confirmação de matrícula enviada para responsáveis do aluno {evento.AlunoId}");
    }

    private async Task RegistrarSistemaPresenca(AlunoMatriculadoEvento evento)
    {
        // Registrar aluno no sistema de controle de presença
        await Task.CompletedTask;
        Console.WriteLine($"[SistemaPresença] Aluno {evento.AlunoId} registrado para controle de frequência");
    }

    private async Task RegistrarLogAuditoria(AlunoMatriculadoEvento evento)
    {
        await Task.CompletedTask;
        Console.WriteLine($"[Auditoria] Matrícula registrada: {evento.MatriculaId} - Aluno {evento.AlunoId} - Turma {evento.TurmaId} - Ano {evento.AnoLetivo}");
    }

    private async Task AtualizarEstatisticasTurma(AlunoMatriculadoEvento evento)
    {
        // Atualizar contador de alunos da turma
        await Task.CompletedTask;
        Console.WriteLine($"[Estatísticas] Contador de alunos atualizado para turma {evento.TurmaId}");
    }
}