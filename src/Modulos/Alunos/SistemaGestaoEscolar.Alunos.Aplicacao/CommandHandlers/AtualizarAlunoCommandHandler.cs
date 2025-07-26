using MediatR;
using SistemaGestaoEscolar.Alunos.Aplicacao.Commands;
using SistemaGestaoEscolar.Alunos.Dominio.Entidades;
using SistemaGestaoEscolar.Alunos.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Alunos.Dominio.Repositorios;
using SistemaGestaoEscolar.Alunos.Dominio.Servicos;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.CommandHandlers;

public class AtualizarAlunoCommandHandler : IRequestHandler<AtualizarAlunoCommand, AtualizarAlunoResponse>
{
    private readonly IRepositorioAluno _repositorioAluno;
    private readonly IServicosDominioAluno _servicosDominio;

    public AtualizarAlunoCommandHandler(
        IRepositorioAluno repositorioAluno,
        IServicosDominioAluno servicosDominio)
    {
        _repositorioAluno = repositorioAluno ?? throw new ArgumentNullException(nameof(repositorioAluno));
        _servicosDominio = servicosDominio ?? throw new ArgumentNullException(nameof(servicosDominio));
    }

    public async Task<AtualizarAlunoResponse> Handle(AtualizarAlunoCommand request, CancellationToken cancellationToken)
    {
        var response = new AtualizarAlunoResponse();

        try
        {
            // Buscar aluno existente com todos os relacionamentos
            var aluno = await _repositorioAluno.ObterCompletoAsync(request.Id);
            if (aluno == null)
            {
                response.Erros.Add("Aluno não encontrado");
                response.Sucesso = false;
                return response;
            }

            // Validar dados
            var errosValidacao = await ValidarCommand(request);
            if (errosValidacao.Any())
            {
                response.Erros = errosValidacao.ToList();
                response.Sucesso = false;
                return response;
            }

            // Atualizar dados do aluno
            var novoNome = new NomeAluno(request.Nome);
            var novoEndereco = new Endereco(
                request.Logradouro,
                request.Numero,
                request.Bairro,
                request.Cidade,
                request.Estado,
                request.Cep,
                request.Complemento);
            var novoGenero = Genero.Criar(request.Genero);
            var novaDeficiencia = request.TipoDeficiencia.HasValue && !string.IsNullOrWhiteSpace(request.DescricaoDeficiencia)
                ? Deficiencia.Criar(request.TipoDeficiencia.Value, request.DescricaoDeficiencia)
                : Deficiencia.Nenhuma();

            aluno.AtualizarNome(novoNome);
            aluno.AtualizarEndereco(novoEndereco);
            aluno.AtualizarGenero(novoGenero);
            aluno.AtualizarDeficiencia(novaDeficiencia);
            aluno.AtualizarTelefone(request.Telefone);
            aluno.AtualizarEmail(request.Email);
            aluno.AtualizarObservacoes(request.Observacoes);

            // Atualizar responsáveis
            if (request.Responsaveis?.Any() == true)
            {
                // Limpar responsáveis existentes
                aluno.LimparResponsaveis();

                // Adicionar novos responsáveis
                foreach (var responsavelRequest in request.Responsaveis)
                {
                    var enderecoResponsavel = !string.IsNullOrWhiteSpace(responsavelRequest.Logradouro)
                        ? new Endereco(
                            responsavelRequest.Logradouro,
                            responsavelRequest.Numero ?? "",
                            responsavelRequest.Bairro ?? "",
                            responsavelRequest.Cidade ?? "",
                            responsavelRequest.Estado ?? "",
                            responsavelRequest.Cep ?? "",
                            responsavelRequest.Complemento)
                        : null;

                    var responsavel = new Responsavel(
                        new NomeAluno(responsavelRequest.Nome),
                        new Cpf(responsavelRequest.Cpf),
                        responsavelRequest.Telefone,
                        (TipoResponsavel)responsavelRequest.Tipo,
                        responsavelRequest.Email,
                        enderecoResponsavel,
                        responsavelRequest.Profissao,
                        responsavelRequest.LocalTrabalho,
                        responsavelRequest.TelefoneTrabalho,
                        responsavelRequest.ResponsavelFinanceiro,
                        responsavelRequest.ResponsavelAcademico,
                        responsavelRequest.AutorizadoBuscar,
                        responsavelRequest.Observacoes);

                    aluno.AdicionarResponsavel(responsavel);
                }
            }

            // Validar dados atualizados (usando serviços puros)
            var errosDominio = _servicosDominio.ValidarDadosAluno(aluno);
            if (errosDominio.Any())
            {
                response.Erros = errosDominio.ToList();
                response.Sucesso = false;
                return response;
            }

            // Salvar alterações
            await _repositorioAluno.AtualizarAsync(aluno);

            // Preparar resposta de sucesso
            response.Id = aluno.Id;
            response.Nome = aluno.Nome.Valor;
            response.DataAtualizacao = DateTime.UtcNow;
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

    private async Task<IEnumerable<string>> ValidarCommand(AtualizarAlunoCommand command)
    {
        var erros = new List<string>();

        // Validações básicas
        if (string.IsNullOrWhiteSpace(command.Nome))
            erros.Add("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(command.Logradouro))
            erros.Add("Logradouro é obrigatório");

        if (string.IsNullOrWhiteSpace(command.Numero))
            erros.Add("Número é obrigatório");

        if (string.IsNullOrWhiteSpace(command.Bairro))
            erros.Add("Bairro é obrigatório");

        if (string.IsNullOrWhiteSpace(command.Cidade))
            erros.Add("Cidade é obrigatória");

        if (string.IsNullOrWhiteSpace(command.Estado))
            erros.Add("Estado é obrigatório");

        if (string.IsNullOrWhiteSpace(command.Cep))
            erros.Add("CEP é obrigatório");

        // Validar email único (se fornecido) - usando repositório diretamente na camada de aplicação
        if (!string.IsNullOrWhiteSpace(command.Email))
        {
            if (await _repositorioAluno.ExisteEmailAsync(command.Email, command.Id))
                erros.Add("Email já está cadastrado para outro aluno");
        }

        return erros;
    }
}