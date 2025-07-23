using SistemaGestaoEscolar.Shared.Domain.Entities;

namespace SistemaGestaoEscolar.Alunos.Dominio.Entidades;

public class Matricula : BaseEntity
{
    public new Guid Id { get; private set; }
    public Guid AlunoId { get; private set; }
    public Guid TurmaId { get; private set; }
    public int AnoLetivo { get; private set; }
    public DateTime DataMatricula { get; private set; }
    public DateTime? DataCancelamento { get; private set; }
    public string? MotivoCancelamento { get; private set; }
    public bool Ativa { get; private set; }
    public string NumeroMatricula { get; private set; } = string.Empty;
    public StatusMatricula Status { get; private set; }
    public string? Observacoes { get; private set; }

    private Matricula() { } // Para EF Core

    public Matricula(
        Guid alunoId,
        Guid turmaId,
        int anoLetivo,
        string? observacoes = null)
    {
        Id = Guid.NewGuid();
        AlunoId = alunoId == Guid.Empty ? throw new ArgumentException("AlunoId não pode ser vazio") : alunoId;
        TurmaId = turmaId == Guid.Empty ? throw new ArgumentException("TurmaId não pode ser vazio") : turmaId;
        AnoLetivo = ValidarAnoLetivo(anoLetivo);
        DataMatricula = DateTime.UtcNow;
        Ativa = true;
        Status = StatusMatricula.Ativa;
        NumeroMatricula = GerarNumeroMatricula();
        Observacoes = string.IsNullOrWhiteSpace(observacoes) ? null : observacoes.Trim();
    }

    public void Inativar(string motivo)
    {
        if (!Ativa)
            throw new InvalidOperationException("Matrícula já está inativa");

        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Motivo do cancelamento é obrigatório");

        Ativa = false;
        Status = StatusMatricula.Cancelada;
        DataCancelamento = DateTime.UtcNow;
        MotivoCancelamento = motivo.Trim();
        MarkAsUpdated();
    }

    public void Reativar()
    {
        if (Ativa)
            throw new InvalidOperationException("Matrícula já está ativa");

        // Verificar se não passou muito tempo desde o cancelamento
        if (DataCancelamento.HasValue && 
            DateTime.UtcNow.Subtract(DataCancelamento.Value).TotalDays > 30)
        {
            throw new InvalidOperationException("Não é possível reativar matrícula cancelada há mais de 30 dias");
        }

        Ativa = true;
        Status = StatusMatricula.Ativa;
        DataCancelamento = null;
        MotivoCancelamento = null;
        MarkAsUpdated();
    }

    public void Transferir(Guid novaTurmaId, string motivo)
    {
        if (!Ativa)
            throw new InvalidOperationException("Não é possível transferir matrícula inativa");

        if (novaTurmaId == Guid.Empty)
            throw new ArgumentException("Nova turma ID não pode ser vazio");

        if (novaTurmaId == TurmaId)
            throw new InvalidOperationException("Aluno já está matriculado nesta turma");

        TurmaId = novaTurmaId;
        Status = StatusMatricula.Transferida;
        
        var observacaoTransferencia = $"Transferido em {DateTime.Now:dd/MM/yyyy}: {motivo}";
        Observacoes = string.IsNullOrEmpty(Observacoes) 
            ? observacaoTransferencia 
            : $"{Observacoes}\n{observacaoTransferencia}";
            
        MarkAsUpdated();
    }

    public void Suspender(string motivo, DateTime? dataRetorno = null)
    {
        if (!Ativa)
            throw new InvalidOperationException("Não é possível suspender matrícula inativa");

        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Motivo da suspensão é obrigatório");

        Status = StatusMatricula.Suspensa;
        
        var observacaoSuspensao = $"Suspenso em {DateTime.Now:dd/MM/yyyy}: {motivo}";
        if (dataRetorno.HasValue)
            observacaoSuspensao += $" (Retorno previsto: {dataRetorno.Value:dd/MM/yyyy})";
            
        Observacoes = string.IsNullOrEmpty(Observacoes) 
            ? observacaoSuspensao 
            : $"{Observacoes}\n{observacaoSuspensao}";
            
        MarkAsUpdated();
    }

    public void RemoverSuspensao()
    {
        if (Status != StatusMatricula.Suspensa)
            throw new InvalidOperationException("Matrícula não está suspensa");

        Status = StatusMatricula.Ativa;
        
        var observacaoRetorno = $"Suspensão removida em {DateTime.Now:dd/MM/yyyy}";
        Observacoes = string.IsNullOrEmpty(Observacoes) 
            ? observacaoRetorno 
            : $"{Observacoes}\n{observacaoRetorno}";
            
        MarkAsUpdated();
    }

    public void AtualizarObservacoes(string? novasObservacoes)
    {
        Observacoes = string.IsNullOrWhiteSpace(novasObservacoes) ? null : novasObservacoes.Trim();
        MarkAsUpdated();
    }

    public bool EstaAtiva()
    {
        return Ativa && Status == StatusMatricula.Ativa;
    }

    public bool EstaSuspensa()
    {
        return Ativa && Status == StatusMatricula.Suspensa;
    }

    public bool FoiTransferida()
    {
        return Status == StatusMatricula.Transferida;
    }

    public bool FoiCancelada()
    {
        return !Ativa && Status == StatusMatricula.Cancelada;
    }

    public int DiasMatriculado()
    {
        var dataFim = DataCancelamento ?? DateTime.UtcNow;
        return (dataFim.Date - DataMatricula.Date).Days;
    }

    public bool EhMatriculaDoAnoCorrente()
    {
        return AnoLetivo == DateTime.Now.Year;
    }

    private static int ValidarAnoLetivo(int anoLetivo)
    {
        var anoAtual = DateTime.Now.Year;
        
        if (anoLetivo < anoAtual - 1 || anoLetivo > anoAtual + 1)
            throw new ArgumentException($"Ano letivo deve estar entre {anoAtual - 1} e {anoAtual + 1}");
            
        return anoLetivo;
    }

    private string GerarNumeroMatricula()
    {
        // Formato: ANO + GUID (8 primeiros caracteres)
        return $"{AnoLetivo}{Id.ToString("N")[..8].ToUpper()}";
    }
}

public enum StatusMatricula
{
    Ativa = 1,
    Suspensa = 2,
    Transferida = 3,
    Cancelada = 4
}

public static class StatusMatriculaExtensions
{
    public static string ObterDescricao(this StatusMatricula status)
    {
        return status switch
        {
            StatusMatricula.Ativa => "Ativa",
            StatusMatricula.Suspensa => "Suspensa",
            StatusMatricula.Transferida => "Transferida",
            StatusMatricula.Cancelada => "Cancelada",
            _ => "Desconhecido"
        };
    }

    public static bool PermiteAlteracoes(this StatusMatricula status)
    {
        return status is StatusMatricula.Ativa or StatusMatricula.Suspensa;
    }
}