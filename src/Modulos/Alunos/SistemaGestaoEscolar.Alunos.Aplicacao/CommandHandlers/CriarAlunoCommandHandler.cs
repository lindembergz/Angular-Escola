using MediatR;
using SistemaGestaoEscolar.Alunos.Aplicacao.Commands;
using SistemaGestaoEscolar.Alunos.Dominio.Entidades;
using SistemaGestaoEscolar.Alunos.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Alunos.Dominio.Repositorios;
using SistemaGestaoEscolar.Alunos.Dominio.Servicos;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Alunos.Aplicacao.CommandHandlers;

public class CriarAlunoCommandHandler : IRequestHandler<CriarAlunoCommand, CriarAlunoResponse>
{
    private readonly IRepositorioAluno _repositorioAluno;
    private readonly IServicosDominioAluno _servicosDominio;

    public CriarAlunoCommandHandler(
        IRepositorioAluno repositorioAluno,
        IServicosDominioAluno servicosDominio)
    {
        _repositorioAluno = repositorioAluno ?? throw new ArgumentNullException(nameof(repositorioAluno));
        _servicosDominio = servicosDominio ?? throw new ArgumentNullException(nameof(servicosDominio));
    }

    public async Task<CriarAlunoResponse> Handle(CriarAlunoCommand request, CancellationToken cancellationToken)
    {
        var response = new CriarAlunoResponse();

        try
        {
            // Validar dados básicos
            var errosValidacao = await ValidarCommand(request);
            if (errosValidacao.Any())
            {
                response.Erros = errosValidacao.ToList();
                response.Sucesso = false;
                return response;
            }

            // Criar objetos de valor
            var nome = new NomeAluno(request.Nome);
            var cpf = new Cpf(request.Cpf);
            var dataNascimento = new DataNascimento(request.DataNascimento);
            var endereco = new Endereco(
                request.Logradouro,
                request.Numero,
                request.Bairro,
                request.Cidade,
                request.Estado,
                request.Cep,
                request.Complemento);
            var genero = Genero.Criar(request.Genero);
            var deficiencia = Deficiencia.Criar(request.TipoDeficiencia, request.DescricaoDeficiencia ?? string.Empty);

            // Criar aluno
            var aluno = new Aluno(
                nome,
                cpf,
                dataNascimento,
                endereco,
                request.EscolaId,
                genero,
                deficiencia,
                request.Telefone,
                request.Email,
                request.Observacoes);

            // Adicionar responsáveis
            foreach (var responsavelDto in request.Responsaveis)
            {
                var responsavel = await CriarResponsavel(responsavelDto);
                aluno.AdicionarResponsavel(responsavel);
            }

            // Validar dados do aluno com responsáveis (usando serviços puros)
            var errosDominio = _servicosDominio.ValidarDadosAluno(aluno);
            if (errosDominio.Any())
            {
                response.Erros = errosDominio.ToList();
                response.Sucesso = false;
                return response;
            }

            // Salvar aluno
            await _repositorioAluno.AdicionarAsync(aluno);

            // Preparar resposta de sucesso
            response.Id = aluno.Id;
            response.Nome = aluno.Nome.Valor;
            response.Cpf = aluno.Cpf.NumeroFormatado;
            response.DataCadastro = aluno.DataCadastro;
            response.Responsaveis = aluno.Responsaveis.Select(r => r.Nome.Valor).ToList();
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

    private async Task<IEnumerable<string>> ValidarCommand(CriarAlunoCommand command)
    {
        var erros = new List<string>();

        // Validações básicas
        if (string.IsNullOrWhiteSpace(command.Nome))
            erros.Add("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(command.Cpf))
            erros.Add("CPF é obrigatório");

        if (command.DataNascimento == default)
            erros.Add("Data de nascimento é obrigatória");

        if (command.EscolaId == Guid.Empty)
            erros.Add("Escola é obrigatória");

        if (!command.Responsaveis.Any())
            erros.Add("Pelo menos um responsável é obrigatório");

        // Validar CPF único (usando repositório diretamente na camada de aplicação)
        if (!string.IsNullOrWhiteSpace(command.Cpf))
        {
            try
            {
                var cpf = new Cpf(command.Cpf);
                if (await _repositorioAluno.ExisteCpfAsync(cpf))
                    erros.Add("CPF já está cadastrado");
            }
            catch (ArgumentException ex)
            {
                erros.Add($"CPF inválido: {ex.Message}");
            }
        }

        // Validar email único (usando repositório diretamente na camada de aplicação)
        if (!string.IsNullOrWhiteSpace(command.Email))
        {
            if (await _repositorioAluno.ExisteEmailAsync(command.Email))
                erros.Add("Email já está cadastrado");
        }

        // Validar responsáveis
        foreach (var responsavel in command.Responsaveis)
        {
            var errosResponsavel = ValidarResponsavel(responsavel);
            erros.AddRange(errosResponsavel);
        }

        return erros;
    }

    private static IEnumerable<string> ValidarResponsavel(CriarResponsavelDto responsavel)
    {
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(responsavel.Nome))
            erros.Add("Nome do responsável é obrigatório");

        if (string.IsNullOrWhiteSpace(responsavel.Cpf))
            erros.Add("CPF do responsável é obrigatório");

        if (string.IsNullOrWhiteSpace(responsavel.Telefone))
            erros.Add("Telefone do responsável é obrigatório");

        if (responsavel.ResponsavelFinanceiro && string.IsNullOrWhiteSpace(responsavel.Email))
            erros.Add("Responsável financeiro deve ter email");

        return erros;
    }

    private static async Task<Responsavel> CriarResponsavel(CriarResponsavelDto dto)
    {
        var nome = new NomeAluno(dto.Nome);
        var cpf = new Cpf(dto.Cpf);
        var tipo = (TipoResponsavel)dto.Tipo;

        Endereco? endereco = null;
        if (!string.IsNullOrWhiteSpace(dto.Logradouro))
        {
            endereco = new Endereco(
                dto.Logradouro,
                dto.Numero ?? "",
                dto.Bairro ?? "",
                dto.Cidade ?? "",
                dto.Estado ?? "",
                dto.Cep ?? "",
                dto.Complemento);
        }

        var responsavel = new Responsavel(
            nome,
            cpf,
            dto.Telefone,
            tipo,
            dto.Email,
            endereco,
            dto.Profissao,
            dto.LocalTrabalho,
            dto.TelefoneTrabalho,
            dto.ResponsavelFinanceiro,
            dto.ResponsavelAcademico,
            dto.AutorizadoBuscar,
            dto.Observacoes);

        await Task.CompletedTask;
        return responsavel;
    }
}