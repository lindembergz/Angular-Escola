using SistemaGestaoEscolar.Alunos.Dominio.Entidades;
using SistemaGestaoEscolar.Alunos.Dominio.ObjetosDeValor;


namespace SistemaGestaoEscolar.Alunos.Dominio.Servicos;

/// <summary>
/// Serviços de domínio puros - contém apenas lógica de negócio sem dependências externas
/// </summary>
public interface IServicosDominioAluno
{
    // ===== MÉTODOS PUROS (sem dependências de repositórios) =====
    
    bool ValidarIdadeEscolar(DataNascimento dataNascimento);
    bool IdadeCompatibilComNivelEnsino(int idade, string nivelEnsino);
    IEnumerable<string> ValidarDadosAluno(Aluno aluno);
    IEnumerable<string> ValidarDadosResponsavel(Responsavel responsavel);
    IEnumerable<string> ValidarRegrasNegocioMatricula(Aluno aluno, int anoLetivo);
    bool PodeTransferirAluno(Aluno aluno);
    bool PodeDesativarAluno(Aluno aluno);
    bool ResponsavelPodeReceberCobrancas(Responsavel responsavel);
    bool ResponsavelPodeReceberBoletins(Responsavel responsavel);
    bool AlunoTemResponsavelFinanceiro(Aluno aluno);
    bool AlunoTemResponsavelAcademico(Aluno aluno);
    bool MatriculaEstaRegular(Matricula matricula);
    IEnumerable<string> ValidarTransferenciaEscola(Aluno aluno, Guid novaEscolaId);
    string GerarNumeroMatricula(int anoLetivo, Guid alunoId);
    Dictionary<string, object> ObterEstatisticasAluno(Aluno aluno);
    bool ValidarCompatibilidadeResponsaveis(IEnumerable<Responsavel> responsaveis);
    IEnumerable<string> ObterPendenciasDocumentais(Aluno aluno);
    bool PodeRenovarMatricula(Aluno aluno, int proximoAnoLetivo);
    int CalcularIdadeNaDataMatricula(DataNascimento dataNascimento, DateTime dataMatricula);
}