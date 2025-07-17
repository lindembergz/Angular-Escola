using SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;
using SistemaGestaoEscolar.Escolas.Dominio.Entidades;
using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Escolas.Dominio.Repositorios;
using SistemaGestaoEscolar.Escolas.Dominio.Servicos;

namespace SistemaGestaoEscolar.Escolas.Aplicacao.CasosDeUso;

public class AdicionarUnidadeEscolarCasoDeUso
{
    private readonly IRepositorioEscola _repositorioEscola;
    private readonly IServicosDominioEscola _servicosDominio;

    public AdicionarUnidadeEscolarCasoDeUso(
        IRepositorioEscola repositorioEscola,
        IServicosDominioEscola servicosDominio)
    {
        _repositorioEscola = repositorioEscola;
        _servicosDominio = servicosDominio;
    }

    public async Task<UnidadeEscolarRespostaDto> ExecutarAsync(AdicionarUnidadeEscolarDto dto)
    {
        // Buscar escola
        var escola = await _repositorioEscola.ObterComUnidadesPorIdAsync(dto.EscolaId);
        if (escola == null)
        {
            throw new InvalidOperationException("Escola não encontrada");
        }

        if (!escola.Ativa)
        {
            throw new InvalidOperationException("Não é possível adicionar unidade a uma escola inativa");
        }

        // Criar objetos de valor
        var nome = new NomeEscola(dto.Nome);
        var endereco = new Endereco(
            dto.Endereco.Logradouro,
            dto.Endereco.Numero,
            dto.Endereco.Bairro,
            dto.Endereco.Cidade,
            dto.Endereco.Estado,
            dto.Endereco.Cep,
            dto.Endereco.Complemento);
        var tipo = new TipoEscola(dto.Tipo);

        // Criar unidade escolar
        var unidade = new UnidadeEscolar(nome, endereco, tipo, dto.EscolaId, dto.CapacidadeMaximaAlunos);

        // Configurações específicas por tipo de ensino
        if (tipo.Valor == "Infantil" && dto.IdadeMinima.HasValue && dto.IdadeMaxima.HasValue)
        {
            unidade.ConfigurarEnsinoInfantil(dto.IdadeMinima.Value, dto.IdadeMaxima.Value, dto.TemBerçario, dto.TemPreEscola);
        }

        // Adicionar séries se fornecidas
        foreach (var serie in dto.SeriesAtendidas)
        {
            if (!string.IsNullOrWhiteSpace(serie))
            {
                unidade.AdicionarSerie(serie);
            }
        }

        // Validações de domínio puras
        var validacoes = _servicosDominio.ValidarRegrasNegocioUnidade(unidade);
        if (validacoes.Any())
        {
            throw new InvalidOperationException($"Erro de validação: {string.Join(", ", validacoes)}");
        }

        // Adicionar unidade à escola
        escola.AdicionarUnidade(unidade);

        // Salvar
        await _repositorioEscola.AtualizarAsync(escola);
        await _repositorioEscola.SaveChangesAsync();

        // Retornar DTO de resposta
        return new UnidadeEscolarRespostaDto
        {
            Id = unidade.Id,
            Nome = unidade.Nome.Valor,
            Endereco = new EnderecoDto
            {
                Logradouro = unidade.Endereco.Logradouro,
                Numero = unidade.Endereco.Numero,
                Complemento = unidade.Endereco.Complemento,
                Bairro = unidade.Endereco.Bairro,
                Cidade = unidade.Endereco.Cidade,
                Estado = unidade.Endereco.Estado,
                Cep = unidade.Endereco.Cep
            },
            Tipo = unidade.Tipo.ToString(),
            CapacidadeMaximaAlunos = unidade.CapacidadeMaximaAlunos,
            AlunosMatriculados = unidade.AlunosMatriculados,
            Ativa = unidade.Ativa,
            DataCriacao = unidade.DataCriacao,
            IdadeMinima = unidade.IdadeMinima,
            IdadeMaxima = unidade.IdadeMaxima,
            TemBerçario = unidade.TemBerçario,
            TemPreEscola = unidade.TemPreEscola,
            SeriesAtendidas = unidade.SeriesAtendidas.ToList(),
            VagasDisponiveis = unidade.VagasDisponiveis,
            PercentualOcupacao = unidade.PercentualOcupacao,
            PodeReceberNovasMatriculas = unidade.PodeReceberNovasMatriculas
        };
    }
}