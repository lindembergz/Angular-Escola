using SistemaGestaoEscolar.Escolas.Dominio.Entidades;
using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;

namespace SistemaGestaoEscolar.Escolas.Dominio.Servicos;

/// <summary>
/// Serviços de domínio puros - contém apenas lógica de negócio sem dependências externas
/// </summary>
public interface IServicosDominioEscola
{
    // ===== MÉTODOS PUROS (sem dependências de repositórios) =====
    
    bool ValidarCapacidadeUnidade(UnidadeEscolar unidade, int novaCapacidade);
    bool PodeDesativarEscola(Escola escola);
    bool PodeRemoverUnidade(UnidadeEscolar unidade);
    int CalcularCapacidadeTotalEscola(Escola escola);
    int CalcularOcupacaoTotalEscola(Escola escola);
    double CalcularPercentualOcupacaoEscola(Escola escola);
    bool ValidarTransferenciaAluno(Escola escolaOrigem, Escola escolaDestino, TipoEscola tipoEnsino);
    IEnumerable<UnidadeEscolar> ObterUnidadesDisponiveisParaMatricula(Escola escola, TipoEscola tipoEnsino);
    bool ValidarCompatibilidadeTipoEnsino(TipoEscola tipoEscola, int idadeAluno);
    string GerarCodigoEscola(NomeEscola nome, string estado, int contador);
    bool ValidarEnderecoEscola(Endereco endereco);
    IEnumerable<string> ValidarRegrasNegocioEscola(Escola escola);
    IEnumerable<string> ValidarRegrasNegocioUnidade(UnidadeEscolar unidade);
    Dictionary<string, object> ObterEstatisticasEscola(Escola escola);
    bool ValidarAssociacaoRede(Escola escola, RedeEscolar rede);
    bool PodeAlterarTipoEnsino(Escola escola, TipoEscola novoTipo);
}