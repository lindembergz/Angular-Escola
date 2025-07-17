using SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;
using SistemaGestaoEscolar.Escolas.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Escolas.Aplicacao.CasosDeUso;

public class ObterEscolasPorRedeCasoDeUso
{
    private readonly IRepositorioEscola _repositorioEscola;
    private readonly IRepositorioRedeEscolar _repositorioRedeEscolar;

    public ObterEscolasPorRedeCasoDeUso(
        IRepositorioEscola repositorioEscola,
        IRepositorioRedeEscolar repositorioRedeEscolar)
    {
        _repositorioEscola = repositorioEscola;
        _repositorioRedeEscolar = repositorioRedeEscolar;
    }

    public async Task<IEnumerable<EscolaRespostaDto>> ExecutarAsync(Guid redeEscolarId)
    {
        // Verificar se a rede existe
        var rede = await _repositorioRedeEscolar.ObterPorIdAsync(redeEscolarId);
        if (rede == null)
        {
            throw new InvalidOperationException("Rede escolar não encontrada");
        }

        // Buscar escolas da rede
        var escolas = await _repositorioEscola.ObterPorRedeAsync(redeEscolarId);

        // Mapear para DTOs
        var escolasDto = new List<EscolaRespostaDto>();
        foreach (var escola in escolas)
        {
            escolasDto.Add(new EscolaRespostaDto
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
                NomeRedeEscolar = rede.Nome.Valor,
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
            });
        }

        return escolasDto;
    }
}