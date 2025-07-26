using SistemaGestaoEscolar.Alunos.Dominio.Entidades;
using SistemaGestaoEscolar.Alunos.Dominio.ObjetosDeValor;


namespace SistemaGestaoEscolar.Alunos.Dominio.Servicos;

public class ServicosDominioAluno : IServicosDominioAluno
{
    // Serviços de domínio puros - SEM dependências externas

    // ===== MÉTODOS PUROS (sem dependências de repositórios) =====

    public bool ValidarIdadeEscolar(DataNascimento dataNascimento)
    {
        return dataNascimento.EhIdadeEscolar();
    }

    public bool IdadeCompatibilComNivelEnsino(int idade, string nivelEnsino)
    {
        return nivelEnsino.ToLower() switch
        {
            "infantil" => idade >= 4 && idade <= 5,
            "fundamental i" => idade >= 6 && idade <= 10,
            "fundamental ii" => idade >= 11 && idade <= 14,
            "médio" => idade >= 15 && idade <= 17,
            _ => idade >= 4 && idade <= 17
        };
    }

    public IEnumerable<string> ValidarDadosAluno(Aluno aluno)
    {
        var erros = new List<string>();

        if (aluno == null)
        {
            erros.Add("Aluno não pode ser nulo");
            return erros;
        }

        // Validar se tem pelo menos um responsável
        if (!aluno.Responsaveis.Any())
            erros.Add("Aluno deve ter pelo menos um responsável");

        // Validar se tem responsável financeiro
        if (!AlunoTemResponsavelFinanceiro(aluno))
            erros.Add("Aluno deve ter pelo menos um responsável financeiro");

        // Validar se tem responsável acadêmico
        if (!AlunoTemResponsavelAcademico(aluno))
            erros.Add("Aluno deve ter pelo menos um responsável acadêmico");

        // Validar idade escolar (warning, não erro)
        if (!ValidarIdadeEscolar(aluno.DataNascimento))
            erros.Add($"Idade do aluno ({aluno.DataNascimento.Idade} anos) está fora da faixa escolar típica");

        // Validar máximo de responsáveis
        if (aluno.Responsaveis.Count > 3)
            erros.Add("Aluno pode ter no máximo 3 responsáveis");

        return erros;
    }

    public IEnumerable<string> ValidarDadosResponsavel(Responsavel responsavel)
    {
        var erros = new List<string>();

        if (responsavel == null)
        {
            erros.Add("Responsável não pode ser nulo");
            return erros;
        }

        // Validar se tem pelo menos um meio de contato
        if (string.IsNullOrEmpty(responsavel.Telefone) && string.IsNullOrEmpty(responsavel.Email))
            erros.Add("Responsável deve ter pelo menos telefone ou email");

        // Validar se responsável financeiro tem email
        if (responsavel.ResponsavelFinanceiro && string.IsNullOrEmpty(responsavel.Email))
            erros.Add("Responsável financeiro deve ter email cadastrado");

        return erros;
    }

    public IEnumerable<string> ValidarRegrasNegocioMatricula(Aluno aluno, int anoLetivo)
    {
        var erros = new List<string>();

        if (aluno == null)
        {
            erros.Add("Aluno não pode ser nulo");
            return erros;
        }

        // Validar se aluno está ativo
        if (!aluno.Ativo)
            erros.Add("Não é possível matricular aluno inativo");

        // Validar se já tem matrícula ativa no ano
        if (aluno.Matriculas.Any(m => m.AnoLetivo == anoLetivo && m.Ativa))
            erros.Add($"Aluno já possui matrícula ativa para o ano letivo {anoLetivo}");

        // Validar ano letivo
        var anoAtual = DateTime.Now.Year;
        if (anoLetivo < anoAtual - 1 || anoLetivo > anoAtual + 1)
            erros.Add($"Ano letivo deve estar entre {anoAtual - 1} e {anoAtual + 1}");

        // Validar se tem responsável financeiro
        if (!AlunoTemResponsavelFinanceiro(aluno))
            erros.Add("Aluno deve ter responsável financeiro para ser matriculado");

        return erros;
    }

    public bool PodeTransferirAluno(Aluno aluno)
    {
        if (aluno == null || !aluno.Ativo)
            return false;

        // Verificar se não tem pendências documentais
        var pendenciasDocumentais = ObterPendenciasDocumentais(aluno);
        return !pendenciasDocumentais.Any();
    }

    public bool PodeDesativarAluno(Aluno aluno)
    {
        if (aluno == null || !aluno.Ativo)
            return false;

        // Pode desativar se não tem matrícula ativa
        return !aluno.PossuiMatriculaAtiva();
    }

    public bool ResponsavelPodeReceberCobrancas(Responsavel responsavel)
    {
        return responsavel?.ResponsavelFinanceiro == true && !string.IsNullOrEmpty(responsavel.Email);
    }

    public bool ResponsavelPodeReceberBoletins(Responsavel responsavel)
    {
        return responsavel?.ResponsavelAcademico == true && !string.IsNullOrEmpty(responsavel.Email);
    }

    public bool AlunoTemResponsavelFinanceiro(Aluno aluno)
    {
        return aluno?.Responsaveis.Any(r => r.ResponsavelFinanceiro) == true;
    }

    public bool AlunoTemResponsavelAcademico(Aluno aluno)
    {
        return aluno?.Responsaveis.Any(r => r.ResponsavelAcademico) == true;
    }

    public bool MatriculaEstaRegular(Matricula matricula)
    {
        return matricula?.EstaAtiva() == true && !matricula.EstaSuspensa();
    }

    public IEnumerable<string> ValidarTransferenciaEscola(Aluno aluno, Guid novaEscolaId)
    {
        var erros = new List<string>();

        if (aluno == null)
        {
            erros.Add("Aluno não pode ser nulo");
            return erros;
        }

        if (!aluno.Ativo)
            erros.Add("Não é possível transferir aluno inativo");

        if (aluno.EscolaId == novaEscolaId)
            erros.Add("A nova escola deve ser diferente da escola atual");

        if (novaEscolaId == Guid.Empty)
            erros.Add("Nova escola deve ser informada");

        // Verificar pendências documentais
        var pendenciasDocumentais = ObterPendenciasDocumentais(aluno);
        if (pendenciasDocumentais.Any())
        {
            erros.Add("Aluno possui pendências documentais que impedem a transferência:");
            erros.AddRange(pendenciasDocumentais);
        }

        return erros;
    }

    public string GerarNumeroMatricula(int anoLetivo, Guid alunoId)
    {
        // Formato: ANO + GUID (8 primeiros caracteres)
        return $"{anoLetivo}{alunoId.ToString("N")[..8].ToUpper()}";
    }

    public Dictionary<string, object> ObterEstatisticasAluno(Aluno aluno)
    {
        if (aluno == null)
            return new Dictionary<string, object>();

        var estatisticas = new Dictionary<string, object>
        {
            ["Idade"] = aluno.DataNascimento.Idade,
            ["FaixaEtariaEscolar"] = aluno.DataNascimento.ObterFaixaEtariaEscolar(),
            ["QuantidadeResponsaveis"] = aluno.Responsaveis.Count,
            ["TemResponsavelFinanceiro"] = AlunoTemResponsavelFinanceiro(aluno),
            ["TemResponsavelAcademico"] = AlunoTemResponsavelAcademico(aluno),
            ["QuantidadeMatriculas"] = aluno.Matriculas.Count,
            ["PossuiMatriculaAtiva"] = aluno.PossuiMatriculaAtiva(),
            ["DiasDesdeUltimaMatricula"] = aluno.Matriculas.Any() ? 
                (DateTime.UtcNow - aluno.Matriculas.Max(m => m.DataMatricula)).Days : 0,
            ["Ativo"] = aluno.Ativo,
            ["DiasDesdeUltimaAtualizacao"] = aluno.UpdatedAt.HasValue ? 
                (DateTime.UtcNow - aluno.UpdatedAt.Value).Days : 
                (DateTime.UtcNow - aluno.CreatedAt).Days
        };

        return estatisticas;
    }

    public bool ValidarCompatibilidadeResponsaveis(IEnumerable<Responsavel> responsaveis)
    {
        var responsaveisList = responsaveis?.ToList() ?? new List<Responsavel>();

        if (!responsaveisList.Any())
            return false;

        // Deve ter pelo menos um responsável financeiro
        if (!responsaveisList.Any(r => r.ResponsavelFinanceiro))
            return false;

        // Deve ter pelo menos um responsável acadêmico
        if (!responsaveisList.Any(r => r.ResponsavelAcademico))
            return false;

        // Não pode ter mais de 3 responsáveis
        if (responsaveisList.Count > 3)
            return false;

        // Não pode ter CPFs duplicados
        var cpfs = responsaveisList.Select(r => r.Cpf.Numero).ToList();
        if (cpfs.Count != cpfs.Distinct().Count())
            return false;

        return true;
    }

    public IEnumerable<string> ObterPendenciasDocumentais(Aluno aluno)
    {
        var pendencias = new List<string>();

        if (aluno == null)
        {
            pendencias.Add("Aluno não encontrado");
            return pendencias;
        }

        // Verificar documentos básicos do aluno
        if (aluno.Cpf == null)
            pendencias.Add("CPF do aluno não informado");

        if (string.IsNullOrEmpty(aluno.Endereco?.Logradouro))
            pendencias.Add("Endereço do aluno incompleto");

        // Verificar responsáveis
        if (!aluno.Responsaveis.Any())
            pendencias.Add("Nenhum responsável cadastrado");
        else
        {
            if (!AlunoTemResponsavelFinanceiro(aluno))
                pendencias.Add("Sem responsável financeiro");

            if (!AlunoTemResponsavelAcademico(aluno))
                pendencias.Add("Sem responsável acadêmico");

            // Verificar se responsáveis financeiros têm email
            var responsaveisFinanceiros = aluno.Responsaveis.Where(r => r.ResponsavelFinanceiro);
            if (responsaveisFinanceiros.Any(r => string.IsNullOrEmpty(r.Email)))
                pendencias.Add("Responsável financeiro sem email cadastrado");
        }

        return pendencias;
    }

    public bool PodeRenovarMatricula(Aluno aluno, int proximoAnoLetivo)
    {
        if (aluno == null || !aluno.Ativo)
            return false;

        // Deve ter matrícula ativa no ano atual
        var anoAtual = DateTime.Now.Year;
        var temMatriculaAtual = aluno.Matriculas.Any(m => 
            m.AnoLetivo == anoAtual && m.EstaAtiva());

        if (!temMatriculaAtual)
            return false;

        // Não pode já ter matrícula para o próximo ano
        var jaTemMatriculaProximoAno = aluno.Matriculas.Any(m => 
            m.AnoLetivo == proximoAnoLetivo);

        if (jaTemMatriculaProximoAno)
            return false;

        // Não pode ter pendências documentais
        var pendenciasDocumentais = ObterPendenciasDocumentais(aluno);
        return !pendenciasDocumentais.Any();
    }

    public int CalcularIdadeNaDataMatricula(DataNascimento dataNascimento, DateTime dataMatricula)
    {
        var idade = dataMatricula.Year - dataNascimento.Valor.Year;

        // Verificar se ainda não fez aniversário na data da matrícula
        if (dataMatricula.Month < dataNascimento.Valor.Month || 
            (dataMatricula.Month == dataNascimento.Valor.Month && dataMatricula.Day < dataNascimento.Valor.Day))
        {
            idade--;
        }

        return idade;
    }
}