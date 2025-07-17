using SistemaGestaoEscolar.Escolas.Dominio.Entidades;
using SistemaGestaoEscolar.Escolas.Dominio.ObjetosDeValor;

namespace SistemaGestaoEscolar.Escolas.Dominio.Servicos;

public class ServicosDominioEscola : IServicosDominioEscola
{
    // Serviços de domínio puros - SEM dependências externas

    // ===== MÉTODOS PUROS (sem dependências de repositórios) =====

    public bool ValidarCapacidadeUnidade(UnidadeEscolar unidade, int novaCapacidade)
    {
        if (novaCapacidade <= 0)
            return false;

        // Não pode reduzir capacidade abaixo do número atual de alunos matriculados
        return novaCapacidade >= unidade.AlunosMatriculados;
    }

    public bool PodeDesativarEscola(Escola escola)
    {
        if (escola == null)
            return false;

        // Não pode desativar se houver alunos matriculados em qualquer unidade
        return escola.Unidades.All(u => u.AlunosMatriculados == 0);
    }

    public bool PodeRemoverUnidade(UnidadeEscolar unidade)
    {
        return unidade != null && unidade.AlunosMatriculados == 0;
    }

    public int CalcularCapacidadeTotalEscola(Escola escola)
    {
        return escola?.Unidades.Where(u => u.Ativa).Sum(u => u.CapacidadeMaximaAlunos) ?? 0;
    }

    public int CalcularOcupacaoTotalEscola(Escola escola)
    {
        return escola?.Unidades.Where(u => u.Ativa).Sum(u => u.AlunosMatriculados) ?? 0;
    }

    public double CalcularPercentualOcupacaoEscola(Escola escola)
    {
        var capacidadeTotal = CalcularCapacidadeTotalEscola(escola);
        if (capacidadeTotal == 0)
            return 0;

        var ocupacaoTotal = CalcularOcupacaoTotalEscola(escola);
        return (double)ocupacaoTotal / capacidadeTotal * 100;
    }

    public bool ValidarTransferenciaAluno(Escola escolaOrigem, Escola escolaDestino, TipoEscola tipoEnsino)
    {
        if (escolaOrigem == null || escolaDestino == null || !escolaDestino.Ativa)
            return false;

        // Verificar se a escola de destino atende o tipo de ensino
        return escolaDestino.Unidades.Any(u => u.Ativa && u.Tipo.Equals(tipoEnsino) && u.PodeReceberNovasMatriculas);
    }

    public IEnumerable<UnidadeEscolar> ObterUnidadesDisponiveisParaMatricula(Escola escola, TipoEscola tipoEnsino)
    {
        if (escola == null)
            return Enumerable.Empty<UnidadeEscolar>();

        return escola.Unidades
            .Where(u => u.Ativa && u.Tipo.Equals(tipoEnsino) && u.PodeReceberNovasMatriculas)
            .OrderBy(u => u.PercentualOcupacao);
    }

    public bool ValidarCompatibilidadeTipoEnsino(TipoEscola tipoEscola, int idadeAluno)
    {
        return tipoEscola.PodeMatricularIdade(idadeAluno);
    }

    public string GerarCodigoEscola(NomeEscola nome, string estado, int contador)
    {
        var iniciais = nome.ObterIniciais();
        return $"{estado}{iniciais}{contador:D3}";
    }

    public bool ValidarEnderecoEscola(Endereco endereco)
    {
        // Validações específicas para endereços de escolas
        // Por exemplo, verificar se não é em área residencial restrita
        return true; // Implementar validações específicas conforme necessário
    }

    public IEnumerable<string> ValidarRegrasNegocioEscola(Escola escola)
    {
        var erros = new List<string>();

        // Validar endereço
        if (!ValidarEnderecoEscola(escola.Endereco))
            erros.Add("Endereço não é válido para uma escola");

        // Validar se escola tem pelo menos uma unidade ativa
        if (escola.Ativa && !escola.Unidades.Any(u => u.Ativa))
            erros.Add("Escola ativa deve ter pelo menos uma unidade ativa");

        return erros;
    }

    public IEnumerable<string> ValidarRegrasNegocioUnidade(UnidadeEscolar unidade)
    {
        var erros = new List<string>();

        // Validar capacidade mínima
        if (unidade.CapacidadeMaximaAlunos < 10)
            erros.Add("Capacidade mínima de uma unidade deve ser 10 alunos");

        // Validar capacidade máxima por tipo
        var capacidadeMaximaPorTipo = unidade.Tipo.Valor switch
        {
            "Infantil" => 200,
            "Fundamental" => 800,
            "Médio" => 600,
            "Fundamental e Médio" => 1200,
            _ => 1000
        };

        if (unidade.CapacidadeMaximaAlunos > capacidadeMaximaPorTipo)
            erros.Add($"Capacidade máxima para {unidade.Tipo.Descricao} é {capacidadeMaximaPorTipo} alunos");

        return erros;
    }

    public Dictionary<string, object> ObterEstatisticasEscola(Escola escola)
    {
        if (escola == null)
            return new Dictionary<string, object>();

        var estatisticas = new Dictionary<string, object>
        {
            ["TotalUnidades"] = escola.Unidades.Count,
            ["UnidadesAtivas"] = escola.Unidades.Count(u => u.Ativa),
            ["CapacidadeTotal"] = CalcularCapacidadeTotalEscola(escola),
            ["OcupacaoTotal"] = CalcularOcupacaoTotalEscola(escola),
            ["PercentualOcupacao"] = CalcularPercentualOcupacaoEscola(escola),
            ["TiposEnsino"] = escola.Unidades.Select(u => u.Tipo.Valor).Distinct().ToList(),
            ["VagasDisponiveis"] = escola.Unidades.Where(u => u.Ativa).Sum(u => u.VagasDisponiveis)
        };

        return estatisticas;
    }

    public bool ValidarAssociacaoRede(Escola escola, RedeEscolar rede)
    {
        if (escola == null || rede == null)
            return false;

        // Escola não pode estar associada a outra rede
        if (escola.RedeEscolarId.HasValue && escola.RedeEscolarId != rede.Id)
            return false;

        // Rede deve estar ativa
        return rede.Ativa;
    }

    public bool PodeAlterarTipoEnsino(Escola escola, TipoEscola novoTipo)
    {
        if (escola == null)
            return false;

        // Não pode alterar se houver alunos matriculados em tipos incompatíveis
        var tiposAtuais = escola.Unidades.Where(u => u.AlunosMatriculados > 0).Select(u => u.Tipo).Distinct();
        
        foreach (var tipoAtual in tiposAtuais)
        {
            if (!TiposCompativeis(tipoAtual, novoTipo))
                return false;
        }

        return true;
    }

    private static bool TiposCompativeis(TipoEscola tipoAtual, TipoEscola novoTipo)
    {
        // Lógica para verificar compatibilidade entre tipos de ensino
        if (tipoAtual.Equals(novoTipo))
            return true;

        // Fundamental pode evoluir para Fundamental e Médio
        if (tipoAtual.Valor == "Fundamental" && novoTipo.Valor == "Fundamental e Médio")
            return true;

        // Médio pode evoluir para Fundamental e Médio
        if (tipoAtual.Valor == "Médio" && novoTipo.Valor == "Fundamental e Médio")
            return true;

        return false;
    }

    // ===== MÉTODOS REMOVIDOS (movidos para Casos de Uso) =====
    // - ValidarCnpjUnicoAsync (precisa de repositório)
    // - ValidarNomeUnicoAsync (precisa de repositório)
    // - NotificarMudancaStatusEscolaAsync (responsabilidade de infraestrutura)
    // - NotificarCapacidadeEsgotadaAsync (responsabilidade de infraestrutura)
    // - VerificarConflitosHorarioAsync (integração com outros módulos)

    // Métodos que precisam de dados externos foram removidos da interface
    // e serão implementados nos Casos de Uso da camada de Aplicação
}