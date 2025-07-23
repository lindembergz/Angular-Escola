using SistemaGestaoEscolar.Alunos.Dominio.Entidades;
using SistemaGestaoEscolar.Shared.Domain.Interfaces;

namespace SistemaGestaoEscolar.Alunos.Dominio.Repositorios;

public interface IRepositorioMatricula : IRepository<Matricula>
{
    Task<Matricula?> ObterPorIdAsync(Guid id);
    Task<Matricula?> ObterPorNumeroAsync(string numeroMatricula);
    Task<IEnumerable<Matricula>> ObterPorAlunoAsync(Guid alunoId);
    Task<IEnumerable<Matricula>> ObterPorTurmaAsync(Guid turmaId);
    Task<IEnumerable<Matricula>> ObterPorAnoLetivoAsync(int anoLetivo);
    Task<IEnumerable<Matricula>> ObterAtivasAsync();
    Task<IEnumerable<Matricula>> ObterInativasAsync();
    Task<IEnumerable<Matricula>> ObterSuspensasAsync();
    Task<IEnumerable<Matricula>> ObterCanceladasAsync();
    Task<Matricula?> ObterMatriculaAtivaDoAlunoAsync(Guid alunoId);
    Task<IEnumerable<Matricula>> ObterMatriculasAtivasPorTurmaAsync(Guid turmaId);
    Task<bool> ExisteMatriculaAtivaAsync(Guid alunoId, int anoLetivo);
    Task<bool> ExisteNumeroMatriculaAsync(string numeroMatricula);
    Task<int> ContarMatriculasAtivasAsync();
    Task<int> ContarMatriculasPorTurmaAsync(Guid turmaId);
    Task<int> ContarMatriculasPorAnoLetivoAsync(int anoLetivo);
    Task AdicionarAsync(Matricula matricula);
    Task AtualizarAsync(Matricula matricula);
    Task RemoverAsync(Matricula matricula);
    
    // Métodos para relatórios e estatísticas
    Task<Dictionary<StatusMatricula, int>> ObterEstatisticasPorStatusAsync();
    Task<Dictionary<int, int>> ObterEstatisticasPorAnoLetivoAsync();
    Task<Dictionary<Guid, int>> ObterEstatisticasPorTurmaAsync();
    Task<IEnumerable<Matricula>> ObterMatriculasRecentesAsync(int dias = 30);
    Task<IEnumerable<Matricula>> ObterCancelamentosRecentesAsync(int dias = 30);
    Task<IEnumerable<Matricula>> ObterTransferenciasRecentesAsync(int dias = 30);
    
    // Métodos para busca avançada
    Task<IEnumerable<Matricula>> BuscarAvancadaAsync(
        Guid? alunoId = null,
        Guid? turmaId = null,
        int? anoLetivo = null,
        StatusMatricula? status = null,
        bool? ativa = null,
        DateTime? dataMatriculaInicio = null,
        DateTime? dataMatriculaFim = null);
        
    // Métodos para paginação
    Task<(IEnumerable<Matricula> Matriculas, int Total)> ObterPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        Guid? filtroTurma = null,
        int? filtroAnoLetivo = null,
        StatusMatricula? filtroStatus = null);
        
    // Métodos específicos para validações
    Task<bool> PodeMatricularAlunoAsync(Guid alunoId, Guid turmaId, int anoLetivo);
    Task<int> ObterCapacidadeDisponivelTurmaAsync(Guid turmaId, int anoLetivo);
    Task<IEnumerable<Matricula>> ObterConflitosMatriculaAsync(Guid alunoId, int anoLetivo);
}