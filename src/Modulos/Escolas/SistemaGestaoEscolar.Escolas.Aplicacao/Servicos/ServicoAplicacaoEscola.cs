using SistemaGestaoEscolar.Escolas.Aplicacao.CasosDeUso;
using SistemaGestaoEscolar.Escolas.Aplicacao.DTOs;
using SistemaGestaoEscolar.Escolas.Dominio.Entidades;
using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Escolas.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Escolas.Aplicacao.Servicos;

public class ServicoAplicacaoEscola : IServicoAplicacaoEscola
{
    private readonly CriarEscolaCasoDeUso _criarEscolaCasoDeUso;
    private readonly CriarRedeEscolarCasoDeUso _criarRedeEscolarCasoDeUso;
    private readonly AdicionarUnidadeEscolarCasoDeUso _adicionarUnidadeEscolarCasoDeUso;
    private readonly ObterEscolasPorRedeCasoDeUso _obterEscolasPorRedeCasoDeUso;
    private readonly IRepositorioEscola _repositorioEscola;
    private readonly IRepositorioRedeEscolar _repositorioRedeEscolar;

    public ServicoAplicacaoEscola(
        CriarEscolaCasoDeUso criarEscolaCasoDeUso,
        CriarRedeEscolarCasoDeUso criarRedeEscolarCasoDeUso,
        AdicionarUnidadeEscolarCasoDeUso adicionarUnidadeEscolarCasoDeUso,
        ObterEscolasPorRedeCasoDeUso obterEscolasPorRedeCasoDeUso,
        IRepositorioEscola repositorioEscola,
        IRepositorioRedeEscolar repositorioRedeEscolar)
    {
        _criarEscolaCasoDeUso = criarEscolaCasoDeUso;
        _criarRedeEscolarCasoDeUso = criarRedeEscolarCasoDeUso;
        _adicionarUnidadeEscolarCasoDeUso = adicionarUnidadeEscolarCasoDeUso;
        _obterEscolasPorRedeCasoDeUso = obterEscolasPorRedeCasoDeUso;
        _repositorioEscola = repositorioEscola;
        _repositorioRedeEscolar = repositorioRedeEscolar;
    }

    // Operações de Escola
    public async Task<EscolaRespostaDto> CriarEscolaAsync(CriarEscolaDto dto)
    {
        return await _criarEscolaCasoDeUso.ExecutarAsync(dto);
    }

    public async Task<EscolaRespostaDto?> ObterEscolaPorIdAsync(Guid id)
    {
        var escola = await _repositorioEscola.ObterComUnidadesPorIdAsync(id);
        if (escola == null) return null;

        return await MapearEscolaParaDto(escola);
    }

    public async Task<IEnumerable<EscolaRespostaDto>> ObterTodasEscolasAsync()
    {
        var escolas = await _repositorioEscola.ObterComUnidadesAsync();
        var escolasDto = new List<EscolaRespostaDto>();

        foreach (var escola in escolas)
        {
            escolasDto.Add(await MapearEscolaParaDto(escola));
        }

        return escolasDto;
    }

    public async Task<IEnumerable<EscolaRespostaDto>> ObterEscolasAtivasAsync()
    {
        var escolas = await _repositorioEscola.ObterAtivasAsync();
        var escolasDto = new List<EscolaRespostaDto>();

        foreach (var escola in escolas)
        {
            escolasDto.Add(await MapearEscolaParaDto(escola));
        }

        return escolasDto;
    }

    public async Task<IEnumerable<EscolaRespostaDto>> ObterEscolasPorTipoAsync(string tipo)
    {
        var tipoEscola = new TipoEscola(tipo);
        var escolas = await _repositorioEscola.ObterPorTipoAsync(tipoEscola);
        var escolasDto = new List<EscolaRespostaDto>();

        foreach (var escola in escolas)
        {
            escolasDto.Add(await MapearEscolaParaDto(escola));
        }

        return escolasDto;
    }

    public async Task<IEnumerable<EscolaRespostaDto>> PesquisarEscolasPorNomeAsync(string nome)
    {
        var escolas = await _repositorioEscola.PesquisarPorNomeAsync(nome);
        var escolasDto = new List<EscolaRespostaDto>();

        foreach (var escola in escolas)
        {
            escolasDto.Add(await MapearEscolaParaDto(escola));
        }

        return escolasDto;
    }

    public async Task<EscolaRespostaDto> AtualizarEscolaAsync(Guid id, CriarEscolaDto dto)
    {
        var escola = await _repositorioEscola.ObterPorIdAsync(id);
        if (escola == null)
            throw new InvalidOperationException("Escola não encontrada");

        // Atualizar propriedades
        if (escola.Nome.Valor != dto.Nome)
        {
            escola.AtualizarNome(new NomeEscola(dto.Nome));
        }

        if (!escola.Endereco.Equals(new Endereco(
            dto.Endereco.Logradouro,
            dto.Endereco.Numero,
            dto.Endereco.Bairro,
            dto.Endereco.Cidade,
            dto.Endereco.Estado,
            dto.Endereco.Cep,
            dto.Endereco.Complemento)))
        {
            escola.AtualizarEndereco(new Endereco(
                dto.Endereco.Logradouro,
                dto.Endereco.Numero,
                dto.Endereco.Bairro,
                dto.Endereco.Cidade,
                dto.Endereco.Estado,
                dto.Endereco.Cep,
                dto.Endereco.Complemento));
        }

        await _repositorioEscola.AtualizarAsync(escola);
        await _repositorioEscola.SaveChangesAsync();

        return await MapearEscolaParaDto(escola);
    }

    public async Task DesativarEscolaAsync(Guid id)
    {
        var escola = await _repositorioEscola.ObterPorIdAsync(id);
        if (escola == null)
            throw new InvalidOperationException("Escola não encontrada");

        escola.Desativar();
        await _repositorioEscola.AtualizarAsync(escola);
        await _repositorioEscola.SaveChangesAsync();
    }

    public async Task AtivarEscolaAsync(Guid id)
    {
        var escola = await _repositorioEscola.ObterPorIdAsync(id);
        if (escola == null)
            throw new InvalidOperationException("Escola não encontrada");

        escola.Ativar();
        await _repositorioEscola.AtualizarAsync(escola);
        await _repositorioEscola.SaveChangesAsync();
    }

    public async Task RemoverEscolaAsync(Guid id)
    {
        var escola = await _repositorioEscola.ObterPorIdAsync(id);
        if (escola == null)
            throw new InvalidOperationException("Escola não encontrada");

        await _repositorioEscola.RemoverAsync(escola);
        await _repositorioEscola.SaveChangesAsync();
    }

    // Operações de Rede Escolar
    public async Task<RedeEscolarRespostaDto> CriarRedeEscolarAsync(CriarRedeEscolarDto dto)
    {
        return await _criarRedeEscolarCasoDeUso.ExecutarAsync(dto);
    }

    public async Task<RedeEscolarRespostaDto?> ObterRedeEscolarPorIdAsync(Guid id)
    {
        var rede = await _repositorioRedeEscolar.ObterComEscolasPorIdAsync(id);
        if (rede == null) return null;

        return MapearRedeEscolarParaDto(rede);
    }

    public async Task<IEnumerable<RedeEscolarRespostaDto>> ObterTodasRedesEscolaresAsync()
    {
        var redes = await _repositorioRedeEscolar.ObterComEscolasAsync();
        return redes.Select(MapearRedeEscolarParaDto);
    }

    public async Task<IEnumerable<RedeEscolarRespostaDto>> ObterRedesEscolaresAtivasAsync()
    {
        var redes = await _repositorioRedeEscolar.ObterAtivasAsync();
        var redesDto = new List<RedeEscolarRespostaDto>();

        foreach (var rede in redes)
        {
            var redeCompleta = await _repositorioRedeEscolar.ObterComEscolasPorIdAsync(rede.Id);
            if (redeCompleta != null)
            {
                redesDto.Add(MapearRedeEscolarParaDto(redeCompleta));
            }
        }

        return redesDto;
    }

    public async Task<RedeEscolarRespostaDto> AtualizarRedeEscolarAsync(Guid id, CriarRedeEscolarDto dto)
    {
        var rede = await _repositorioRedeEscolar.ObterPorIdAsync(id);
        if (rede == null)
            throw new InvalidOperationException("Rede escolar não encontrada");

        // Atualizar propriedades
        if (rede.Nome.Valor != dto.Nome)
        {
            rede.AtualizarNome(new NomeEscola(dto.Nome));
        }

        if (!rede.EnderecoSede.Equals(new Endereco(
            dto.EnderecoSede.Logradouro,
            dto.EnderecoSede.Numero,
            dto.EnderecoSede.Bairro,
            dto.EnderecoSede.Cidade,
            dto.EnderecoSede.Estado,
            dto.EnderecoSede.Cep,
            dto.EnderecoSede.Complemento)))
        {
            rede.AtualizarEnderecoSede(new Endereco(
                dto.EnderecoSede.Logradouro,
                dto.EnderecoSede.Numero,
                dto.EnderecoSede.Bairro,
                dto.EnderecoSede.Cidade,
                dto.EnderecoSede.Estado,
                dto.EnderecoSede.Cep,
                dto.EnderecoSede.Complemento));
        }

        await _repositorioRedeEscolar.AtualizarAsync(rede);
        await _repositorioRedeEscolar.SaveChangesAsync();

        return MapearRedeEscolarParaDto(rede);
    }

    public async Task DesativarRedeEscolarAsync(Guid id)
    {
        var rede = await _repositorioRedeEscolar.ObterComEscolasPorIdAsync(id);
        if (rede == null)
            throw new InvalidOperationException("Rede escolar não encontrada");

        rede.Desativar();
        await _repositorioRedeEscolar.AtualizarAsync(rede);
        await _repositorioRedeEscolar.SaveChangesAsync();
    }

    public async Task AtivarRedeEscolarAsync(Guid id)
    {
        var rede = await _repositorioRedeEscolar.ObterPorIdAsync(id);
        if (rede == null)
            throw new InvalidOperationException("Rede escolar não encontrada");

        rede.Ativar();
        await _repositorioRedeEscolar.AtualizarAsync(rede);
        await _repositorioRedeEscolar.SaveChangesAsync();
    }

    public async Task RemoverRedeEscolarAsync(Guid id)
    {
        var rede = await _repositorioRedeEscolar.ObterPorIdAsync(id);
        if (rede == null)
            throw new InvalidOperationException("Rede escolar não encontrada");

        await _repositorioRedeEscolar.RemoverAsync(rede);
        await _repositorioRedeEscolar.SaveChangesAsync();
    }

    // Operações de Unidade Escolar
    public async Task<UnidadeEscolarRespostaDto> AdicionarUnidadeEscolarAsync(AdicionarUnidadeEscolarDto dto)
    {
        return await _adicionarUnidadeEscolarCasoDeUso.ExecutarAsync(dto);
    }

    public async Task RemoverUnidadeEscolarAsync(Guid escolaId, Guid unidadeId)
    {
        var escola = await _repositorioEscola.ObterComUnidadesPorIdAsync(escolaId);
        if (escola == null)
            throw new InvalidOperationException("Escola não encontrada");

        escola.RemoverUnidade(unidadeId);
        await _repositorioEscola.AtualizarAsync(escola);
        await _repositorioEscola.SaveChangesAsync();
    }

    // Operações de Relacionamento
    public async Task<IEnumerable<EscolaRespostaDto>> ObterEscolasPorRedeAsync(Guid redeEscolarId)
    {
        return await _obterEscolasPorRedeCasoDeUso.ExecutarAsync(redeEscolarId);
    }

    public async Task AssociarEscolaRedeAsync(Guid escolaId, Guid redeEscolarId)
    {
        var escola = await _repositorioEscola.ObterPorIdAsync(escolaId);
        if (escola == null)
            throw new InvalidOperationException("Escola não encontrada");

        var rede = await _repositorioRedeEscolar.ObterPorIdAsync(redeEscolarId);
        if (rede == null)
            throw new InvalidOperationException("Rede escolar não encontrada");

        escola.AssociarRede(redeEscolarId);
        await _repositorioEscola.AtualizarAsync(escola);
        await _repositorioEscola.SaveChangesAsync();
    }

    public async Task DesassociarEscolaRedeAsync(Guid escolaId)
    {
        var escola = await _repositorioEscola.ObterPorIdAsync(escolaId);
        if (escola == null)
            throw new InvalidOperationException("Escola não encontrada");

        escola.DesassociarRede();
        await _repositorioEscola.AtualizarAsync(escola);
        await _repositorioEscola.SaveChangesAsync();
    }

    // Operações de Consulta Avançada
    public async Task<(IEnumerable<EscolaRespostaDto> Escolas, int Total)> ObterEscolasPaginadoAsync(
        int pagina, int tamanhoPagina, string? filtroNome = null, string? filtroTipo = null, bool? filtroAtiva = null)
    {
        TipoEscola? tipo = null;
        if (!string.IsNullOrEmpty(filtroTipo))
        {
            tipo = new TipoEscola(filtroTipo);
        }

        var (escolas, total) = await _repositorioEscola.ObterPaginadoAsync(pagina, tamanhoPagina, filtroNome, tipo, filtroAtiva);
        
        var escolasDto = new List<EscolaRespostaDto>();
        foreach (var escola in escolas)
        {
            escolasDto.Add(await MapearEscolaParaDto(escola));
        }

        return (escolasDto, total);
    }

    public async Task<(IEnumerable<RedeEscolarRespostaDto> Redes, int Total)> ObterRedesEscolaresPaginadoAsync(
        int pagina, int tamanhoPagina, string? filtroNome = null, bool? filtroAtiva = null)
    {
        var (redes, total) = await _repositorioRedeEscolar.ObterPaginadoAsync(pagina, tamanhoPagina, filtroNome, null, filtroAtiva);
        
        var redesDto = new List<RedeEscolarRespostaDto>();
        foreach (var rede in redes)
        {
            var redeCompleta = await _repositorioRedeEscolar.ObterComEscolasPorIdAsync(rede.Id);
            if (redeCompleta != null)
            {
                redesDto.Add(MapearRedeEscolarParaDto(redeCompleta));
            }
        }

        return (redesDto, total);
    }

    // Estatísticas
    public async Task<Dictionary<string, int>> ObterEstatisticasEscolasPorTipoAsync()
    {
        return await _repositorioEscola.ObterEstatisticasPorTipoAsync();
    }

    public async Task<Dictionary<string, int>> ObterEstatisticasEscolasPorEstadoAsync()
    {
        return await _repositorioEscola.ObterEstatisticasPorEstadoAsync();
    }

    public async Task<int> ObterTotalEscolasAtivasAsync()
    {
        return await _repositorioEscola.ContarEscolasAtivasAsync();
    }

    public async Task<int> ObterTotalRedesEscolaresAtivasAsync()
    {
        return await _repositorioRedeEscolar.ContarRedesAtivasAsync();
    }

    // Métodos auxiliares de mapeamento
    private async Task<EscolaRespostaDto> MapearEscolaParaDto(Escola escola)
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

    private static RedeEscolarRespostaDto MapearRedeEscolarParaDto(RedeEscolar rede)
    {
        return new RedeEscolarRespostaDto
        {
            Id = rede.Id,
            Nome = rede.Nome.Valor,
            CnpjMantenedora = rede.CnpjMantenedora.Numero,
            EnderecoSede = new EnderecoDto
            {
                Logradouro = rede.EnderecoSede.Logradouro,
                Numero = rede.EnderecoSede.Numero,
                Complemento = rede.EnderecoSede.Complemento,
                Bairro = rede.EnderecoSede.Bairro,
                Cidade = rede.EnderecoSede.Cidade,
                Estado = rede.EnderecoSede.Estado,
                Cep = rede.EnderecoSede.Cep
            },
            DataCriacao = rede.DataCriacao,
            Ativa = rede.Ativa,
            TotalEscolas = rede.TotalEscolas,
            EscolasAtivas = rede.EscolasAtivas,
            EscolasInativas = rede.EscolasInativas,
            Escolas = rede.Escolas.Select(e => new EscolaRespostaDto
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