using SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;
using SistemaGestaoEscolar.Escolas.Dominio.Entidades;
using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Escolas.Dominio.Repositorios;
using SistemaGestaoEscolar.Escolas.Dominio.Servicos;

namespace SistemaGestaoEscolar.Escolas.Aplicacao.CasosDeUso;

public class CriarEscolaCasoDeUso
{
    private readonly IRepositorioEscola _repositorioEscola;
    private readonly IRepositorioRedeEscolar _repositorioRedeEscolar;
    private readonly IServicosDominioEscola _servicosDominio;

    public CriarEscolaCasoDeUso(
        IRepositorioEscola repositorioEscola,
        IRepositorioRedeEscolar repositorioRedeEscolar,
        IServicosDominioEscola servicosDominio)
    {
        _repositorioEscola = repositorioEscola;
        _repositorioRedeEscolar = repositorioRedeEscolar;
        _servicosDominio = servicosDominio;
    }

    public async Task<EscolaRespostaDto> ExecutarAsync(CriarEscolaDto dto)
    {
        // Validar se CNPJ já existe
        var cnpj = new CNPJ(dto.Cnpj);
        if (await _repositorioEscola.ExisteCnpjAsync(cnpj))
        {
            throw new InvalidOperationException($"Já existe uma escola cadastrada com o CNPJ {dto.Cnpj}");
        }

        // Validar se nome já existe
        var nome = new NomeEscola(dto.Nome);
        if (await _repositorioEscola.ExisteNomeAsync(nome))
        {
            throw new InvalidOperationException($"Já existe uma escola cadastrada com o nome {dto.Nome}");
        }

        // Validar rede escolar se informada
        if (dto.RedeEscolarId.HasValue)
        {
            var rede = await _repositorioRedeEscolar.ObterPorIdAsync(dto.RedeEscolarId.Value);
            if (rede == null)
            {
                throw new InvalidOperationException("Rede escolar não encontrada");
            }

            if (!rede.Ativa)
            {
                throw new InvalidOperationException("Não é possível associar escola a uma rede inativa");
            }
        }

        // Criar objetos de valor
        var endereco = new Endereco(
            dto.Endereco.Logradouro,
            dto.Endereco.Numero,
            dto.Endereco.Bairro,
            dto.Endereco.Cidade,
            dto.Endereco.Estado,
            dto.Endereco.Cep,
            dto.Endereco.Complemento);

        var tipo = new TipoEscola(dto.Tipo);

        // Criar escola
        var escola = new Escola(nome, cnpj, endereco, tipo, dto.RedeEscolarId);

        // Validações de domínio puras
        var validacoes = _servicosDominio.ValidarRegrasNegocioEscola(escola);
        if (validacoes.Any())
        {
            throw new InvalidOperationException($"Erro de validação: {string.Join(", ", validacoes)}");
        }

        // Salvar
        await _repositorioEscola.AdicionarAsync(escola);
        await _repositorioEscola.SaveChangesAsync();

        // Retornar DTO de resposta
        return await MapearParaRespostaDto(escola);
    }

    private async Task<EscolaRespostaDto> MapearParaRespostaDto(Escola escola)
    {
        string? nomeRede = null;
        if (escola.RedeEscolarId.HasValue)
        {
            var rede = await _repositorioRedeEscolar.ObterPorIdAsync(escola.RedeEscolarId.Value);
            nomeRede = rede?.Nome.Valor;
        }

        return new EscolaRespostaDto
        {
            Id = escola.Id,
            Nome = escola.Nome.Valor,
            Cnpj = escola.Cnpj.Numero,
            Endereco = new EnderecoDto
            {
                Logradouro = escola.Endereco.Logradouro,
                Numero = escola.Endereco.Numero,
                Complemento = escola.Endereco.Complemento,
                Bairro = escola.Endereco.Bairro,
                Cidade = escola.Endereco.Cidade,
                Estado = escola.Endereco.Estado,
                Cep = escola.Endereco.Cep
            },
            Tipo = escola.Tipo.ToString(),
            RedeEscolarId = escola.RedeEscolarId,
            NomeRedeEscolar = nomeRede,
            DataCriacao = escola.DataCriacao,
            Ativa = escola.Ativa,
            Unidades = escola.Unidades.Select(u => new UnidadeEscolarRespostaDto
            {
                Id = u.Id,
                Nome = u.Nome.Valor,
                Endereco = new EnderecoDto
                {
                    Logradouro = u.Endereco.Logradouro,
                    Numero = u.Endereco.Numero,
                    Complemento = u.Endereco.Complemento,
                    Bairro = u.Endereco.Bairro,
                    Cidade = u.Endereco.Cidade,
                    Estado = u.Endereco.Estado,
                    Cep = u.Endereco.Cep
                },
                Tipo = u.Tipo.ToString(),
                CapacidadeMaximaAlunos = u.CapacidadeMaximaAlunos,
                AlunosMatriculados = u.AlunosMatriculados,
                Ativa = u.Ativa,
                DataCriacao = u.DataCriacao,
                IdadeMinima = u.IdadeMinima,
                IdadeMaxima = u.IdadeMaxima,
                TemBerçario = u.TemBerçario,
                TemPreEscola = u.TemPreEscola,
                SeriesAtendidas = u.SeriesAtendidas.ToList(),
                VagasDisponiveis = u.VagasDisponiveis,
                PercentualOcupacao = u.PercentualOcupacao,
                PodeReceberNovasMatriculas = u.PodeReceberNovasMatriculas
            }).ToList()
        };
    }
}