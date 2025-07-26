using SistemaGestaoEscolar.Alunos.Dominio.Entidades;
using SistemaGestaoEscolar.Shared.Domain.Interfaces;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Alunos.Dominio.Repositorios;

public interface IRepositorioAluno : IRepository<Aluno>
{
    Task<Aluno?> ObterPorIdAsync(Guid id);
    Task<Aluno?> ObterPorCpfAsync(Cpf cpf);
    Task<IEnumerable<Aluno>> ObterPorEscolaAsync(Guid escolaId);
    Task<IEnumerable<Aluno>> ObterAtivosAsync();
    Task<IEnumerable<Aluno>> ObterInativosAsync();
    Task<IEnumerable<Aluno>> PesquisarPorNomeAsync(string nome);
    Task<IEnumerable<Aluno>> ObterPorIdadeAsync(int idadeMinima, int idadeMaxima);
    Task<IEnumerable<Aluno>> ObterPorFaixaEtariaAsync(string faixaEtaria);
    Task<bool> ExisteCpfAsync(Cpf cpf);
    Task<bool> ExisteEmailAsync(string email, Guid? excluirId = null);
    Task<int> ContarAlunosAtivosAsync();
    Task<int> ContarAlunosPorEscolaAsync(Guid escolaId);
    Task<int> ContarAlunosPorIdadeAsync(int idade);
    Task AdicionarAsync(Aluno aluno);
    Task AtualizarAsync(Aluno aluno);
    Task RemoverAsync(Aluno aluno);
    
    // Métodos com relacionamentos
    Task<Aluno?> ObterComResponsaveisPorIdAsync(Guid id);
    Task<Aluno?> ObterComMatriculasPorIdAsync(Guid id);
    Task<Aluno?> ObterCompletoAsync(Guid id);
    Task<IEnumerable<Aluno>> ObterComResponsaveisAsync();
    Task<IEnumerable<Aluno>> ObterComMatriculasAtivasAsync();
    
    // Métodos para relatórios e estatísticas
    Task<Dictionary<string, int>> ObterEstatisticasPorIdadeAsync();
    Task<Dictionary<string, int>> ObterEstatisticasPorEscolaAsync();
    Task<Dictionary<string, int>> ObterEstatisticasPorFaixaEtariaAsync();
    Task<IEnumerable<Aluno>> ObterAniversariantesDoMesAsync(int mes);
    Task<IEnumerable<Aluno>> ObterSemResponsavelFinanceiroAsync();
    Task<IEnumerable<Aluno>> ObterSemMatriculaAtivaAsync();
    
    // Métodos para busca avançada
    Task<IEnumerable<Aluno>> BuscarAvancadaAsync(
        string? nome = null,
        Cpf? cpf = null,
        Guid? escolaId = null,
        int? idadeMinima = null,
        int? idadeMaxima = null,
        string? cidade = null,
        string? estado = null,
        bool? ativo = null,
        bool? possuiMatriculaAtiva = null);
        
    // Métodos para paginação
    Task<(IEnumerable<Aluno> Alunos, int Total)> ObterPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        string? filtroNome = null,
        Guid? filtroEscola = null,
        bool? filtroAtivo = null);
        
    // Métodos específicos para responsáveis
    Task<IEnumerable<Aluno>> ObterPorResponsavelAsync(Guid responsavelId);
    Task<IEnumerable<Aluno>> ObterPorCpfResponsavelAsync(Cpf cpfResponsavel);
    Task<IEnumerable<Aluno>> ObterPorEmailResponsavelAsync(string emailResponsavel);
}