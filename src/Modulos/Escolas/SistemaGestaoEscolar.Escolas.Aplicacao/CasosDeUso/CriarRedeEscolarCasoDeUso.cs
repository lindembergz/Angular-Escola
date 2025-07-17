using SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;
using SistemaGestaoEscolar.Escolas.Dominio.Entidades;
using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Escolas.Dominio.Repositorios;
using SistemaGestaoEscolar.Escolas.Dominio.Servicos;

namespace SistemaGestaoEscolar.Escolas.Aplicacao.CasosDeUso;

public class CriarRedeEscolarCasoDeUso
{
    private readonly IRepositorioRedeEscolar _repositorioRedeEscolar;
    private readonly IServicosDominioEscola _servicosDominio;

    public CriarRedeEscolarCasoDeUso(
        IRepositorioRedeEscolar repositorioRedeEscolar,
        IServicosDominioEscola servicosDominio)
    {
        _repositorioRedeEscolar = repositorioRedeEscolar;
        _servicosDominio = servicosDominio;
    }

    public async Task<RedeEscolarRespostaDto> ExecutarAsync(CriarRedeEscolarDto dto)
    {
        // Validar se CNPJ já existe
        var cnpjMantenedora = new CNPJ(dto.CnpjMantenedora);
        if (await _repositorioRedeEscolar.ExisteCnpjAsync(cnpjMantenedora))
        {
            throw new InvalidOperationException($"Já existe uma rede escolar cadastrada com o CNPJ {dto.CnpjMantenedora}");
        }

        // Validar se nome já existe
        var nome = new NomeEscola(dto.Nome);
        if (await _repositorioRedeEscolar.ExisteNomeAsync(nome))
        {
            throw new InvalidOperationException($"Já existe uma rede escolar cadastrada com o nome {dto.Nome}");
        }

        // Criar objetos de valor
        var enderecoSede = new Endereco(
            dto.EnderecoSede.Logradouro,
            dto.EnderecoSede.Numero,
            dto.EnderecoSede.Bairro,
            dto.EnderecoSede.Cidade,
            dto.EnderecoSede.Estado,
            dto.EnderecoSede.Cep,
            dto.EnderecoSede.Complemento);

        // Criar rede escolar
        var redeEscolar = new RedeEscolar(nome, cnpjMantenedora, enderecoSede);

        // Validações de domínio puras
        var endereco = new Endereco(
            dto.EnderecoSede.Logradouro,
            dto.EnderecoSede.Numero,
            dto.EnderecoSede.Bairro,
            dto.EnderecoSede.Cidade,
            dto.EnderecoSede.Estado,
            dto.EnderecoSede.Cep,
            dto.EnderecoSede.Complemento);

        if (!_servicosDominio.ValidarEnderecoEscola(endereco))
        {
            throw new InvalidOperationException("Endereço da sede não é válido");
        }

        // Salvar
        await _repositorioRedeEscolar.AdicionarAsync(redeEscolar);
        await _repositorioRedeEscolar.SaveChangesAsync();

        // Retornar DTO de resposta
        return MapearParaRespostaDto(redeEscolar);
    }

    private static RedeEscolarRespostaDto MapearParaRespostaDto(RedeEscolar redeEscolar)
    {
        return new RedeEscolarRespostaDto
        {
            Id = redeEscolar.Id,
            Nome = redeEscolar.Nome.Valor,
            CnpjMantenedora = redeEscolar.CnpjMantenedora.Numero,
            EnderecoSede = new EnderecoDto
            {
                Logradouro = redeEscolar.EnderecoSede.Logradouro,
                Numero = redeEscolar.EnderecoSede.Numero,
                Complemento = redeEscolar.EnderecoSede.Complemento,
                Bairro = redeEscolar.EnderecoSede.Bairro,
                Cidade = redeEscolar.EnderecoSede.Cidade,
                Estado = redeEscolar.EnderecoSede.Estado,
                Cep = redeEscolar.EnderecoSede.Cep
            },
            DataCriacao = redeEscolar.DataCriacao,
            Ativa = redeEscolar.Ativa,
            TotalEscolas = redeEscolar.TotalEscolas,
            EscolasAtivas = redeEscolar.EscolasAtivas,
            EscolasInativas = redeEscolar.EscolasInativas,
            Escolas = redeEscolar.Escolas.Select(e => new EscolaRespostaDto
            {
                Id = e.Id,
                Nome = e.Nome.Valor,
                Cnpj = e.Cnpj.Numero,
                Endereco = new EnderecoDto
                {
                    Logradouro = e.Endereco.Logradouro,
                    Numero = e.Endereco.Numero,
                    Complemento = e.Endereco.Complemento,
                    Bairro = e.Endereco.Bairro,
                    Cidade = e.Endereco.Cidade,
                    Estado = e.Endereco.Estado,
                    Cep = e.Endereco.Cep
                },
                Tipo = e.Tipo.ToString(),
                RedeEscolarId = e.RedeEscolarId,
                DataCriacao = e.DataCriacao,
                Ativa = e.Ativa
            }).ToList()
        };
    }
}