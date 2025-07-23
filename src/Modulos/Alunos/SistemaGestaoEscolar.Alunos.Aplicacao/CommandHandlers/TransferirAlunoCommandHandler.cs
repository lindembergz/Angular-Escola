using MediatR;
using SistemaGestaoEscolar.Alunos.Aplicacao.Commands;
using SistemaGestaoEscolar.Alunos.Dominio.Repositorios;
using SistemaGestaoEscolar.Alunos.Dominio.Servicos;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.CommandHandlers;

public class TransferirAlunoCommandHandler : IRequestHandler<TransferirAlunoCommand, TransferirAlunoResponse>
{
    private readonly IRepositorioAluno _repositorioAluno;
    private readonly IServicosDominioAluno _servicosDominio;

    public TransferirAlunoCommandHandler(
        IRepositorioAluno repositorioAluno,
        IServicosDominioAluno servicosDominio)
    {
        _repositorioAluno = repositorioAluno ?? throw new ArgumentNullException(nameof(repositorioAluno));
        _servicosDominio = servicosDominio ?? throw new ArgumentNullException(nameof(servicosDominio));
    }

    public async Task<TransferirAlunoResponse> Handle(TransferirAlunoCommand request, CancellationToken cancellationToken)
    {
        var response = new TransferirAlunoResponse();

        try
        {
            // Buscar aluno existente
            var aluno = await _repositorioAluno.ObterCompletoAsync(request.AlunoId);
            if (aluno == null)
            {
                response.Erros.Add("Aluno não encontrado");
                response.Sucesso = false;
                return response;
            }

            // Validar dados
            var errosValidacao = await ValidarCommand(request, aluno.EscolaId);
            if (errosValidacao.Any())
            {
                response.Erros = errosValidacao.ToList();
                response.Sucesso = false;
                return response;
            }

            // Verificar se pode transferir (usando serviços puros)
            if (!_servicosDominio.PodeTransferirAluno(aluno))
            {
                var pendencias = _servicosDominio.ObterPendenciasDocumentais(aluno);
                response.Erros.Add("Não é possível transferir o aluno devido a pendências:");
                response.Erros.AddRange(pendencias);
                response.Sucesso = false;
                return response;
            }

            // Validar regras de transferência
            var errosTransferencia = _servicosDominio.ValidarTransferenciaEscola(aluno, request.NovaEscolaId);
            if (errosTransferencia.Any())
            {
                response.Erros.AddRange(errosTransferencia);
                response.Sucesso = false;
                return response;
            }

            // Realizar transferência
            var escolaAnterior = aluno.EscolaId;
            aluno.TransferirEscola(request.NovaEscolaId, request.Motivo);

            // Salvar alterações
            await _repositorioAluno.AtualizarAsync(aluno);

            // TODO: Notificar responsáveis sobre a transferência
            // Esta funcionalidade será implementada via Event Handlers

            // Preparar resposta de sucesso
            response.AlunoId = aluno.Id;
            response.NomeAluno = aluno.Nome.Valor;
            response.EscolaAnteriorId = escolaAnterior;
            response.NovaEscolaId = request.NovaEscolaId;
            response.Motivo = request.Motivo;
            response.DataTransferencia = request.DataTransferencia ?? DateTime.UtcNow;
            response.Sucesso = true;

            return response;
        }
        catch (Exception ex)
        {
            response.Erros.Add($"Erro interno: {ex.Message}");
            response.Sucesso = false;
            return response;
        }
    }

    private static async Task<IEnumerable<string>> ValidarCommand(TransferirAlunoCommand command, Guid escolaAtualId)
    {
        var erros = new List<string>();

        if (command.AlunoId == Guid.Empty)
            erros.Add("ID do aluno é obrigatório");

        if (command.NovaEscolaId == Guid.Empty)
            erros.Add("ID da nova escola é obrigatório");

        if (string.IsNullOrWhiteSpace(command.Motivo))
            erros.Add("Motivo da transferência é obrigatório");

        if (command.NovaEscolaId == escolaAtualId)
            erros.Add("A nova escola deve ser diferente da escola atual");

        await Task.CompletedTask;
        return erros;
    }
}