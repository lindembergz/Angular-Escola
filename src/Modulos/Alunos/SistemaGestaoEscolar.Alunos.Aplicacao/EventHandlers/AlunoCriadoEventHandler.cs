using MediatR;
using SistemaGestaoEscolar.Alunos.Dominio.Eventos;
using SistemaGestaoEscolar.Shared.Domain.Events;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.EventHandlers;

public class AlunoCriadoEventHandler : INotificationHandler<AlunoCriadoEvento>
{
    // Aqui seriam injetados serviços para integração com outros módulos
    // Por exemplo: IServicoNotificacao, IServicoFinanceiro, etc.

    public AlunoCriadoEventHandler()
    {
        // Injeção de dependências seria feita aqui
    }

    public async Task Handle(AlunoCriadoEvento notification, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Notificar módulo financeiro para criar cobrança inicial
            await NotificarModuloFinanceiro(notification);

            // 2. Enviar email de boas-vindas para responsáveis
            await EnviarEmailBoasVindas(notification);

            // 3. Registrar log de auditoria
            await RegistrarLogAuditoria(notification);

            // 4. Atualizar estatísticas da escola
            await AtualizarEstatisticasEscola(notification);
        }
        catch (Exception ex)
        {
            // Log do erro - não deve falhar o processo principal
            Console.WriteLine($"Erro ao processar evento AlunoCriado: {ex.Message}");
        }
    }

    private async Task NotificarModuloFinanceiro(AlunoCriadoEvento evento)
    {
        // Integração com módulo financeiro
        // Exemplo: criar mensalidade inicial, configurar plano de pagamento
        await Task.CompletedTask;
        
        Console.WriteLine($"[FinanceiroIntegração] Aluno {evento.Nome} criado - configurando cobrança");
    }

    private async Task EnviarEmailBoasVindas(AlunoCriadoEvento evento)
    {
        // Integração com serviço de email
        // Buscar responsáveis e enviar email de boas-vindas
        await Task.CompletedTask;
        
        Console.WriteLine($"[EmailService] Email de boas-vindas enviado para responsáveis do aluno {evento.Nome}");
    }

    private async Task RegistrarLogAuditoria(AlunoCriadoEvento evento)
    {
        // Registrar no sistema de auditoria
        await Task.CompletedTask;
        
        Console.WriteLine($"[Auditoria] Aluno criado: {evento.Nome} (CPF: {evento.Cpf}) na escola {evento.EscolaId}");
    }

    private async Task AtualizarEstatisticasEscola(AlunoCriadoEvento evento)
    {
        // Atualizar contadores e estatísticas da escola
        await Task.CompletedTask;
        
        Console.WriteLine($"[Estatísticas] Contador de alunos atualizado para escola {evento.EscolaId}");
    }
}