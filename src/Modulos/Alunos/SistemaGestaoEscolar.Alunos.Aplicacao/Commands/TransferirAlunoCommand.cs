using MediatR;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.Commands;

public class TransferirAlunoCommand : IRequest<TransferirAlunoResponse>
{
    public Guid AlunoId { get; set; }
    public Guid NovaEscolaId { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public DateTime? DataTransferencia { get; set; }
    public string? Observacoes { get; set; }
}

public class TransferirAlunoResponse
{
    public Guid AlunoId { get; set; }
    public string NomeAluno { get; set; } = string.Empty;
    public Guid EscolaAnteriorId { get; set; }
    public Guid NovaEscolaId { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public DateTime DataTransferencia { get; set; }
    public bool Sucesso { get; set; }
    public List<string> Erros { get; set; } = new();
}