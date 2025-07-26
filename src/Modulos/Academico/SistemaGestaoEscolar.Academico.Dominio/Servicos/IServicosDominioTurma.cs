using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;

namespace SistemaGestaoEscolar.Academico.Dominio.Servicos;

public interface IServicosDominioTurma
{
    Task<bool> PodeMatricularAlunoAsync(Guid turmaId, Guid alunoId);
    Task<bool> PodeCriarTurmaAsync(string nome, int anoLetivo, Guid escolaId);
    Task<int> CalcularCapacidadeSugeridaAsync(Serie serie, TipoTurno tipoTurno);
    Task<bool> PodeInativarTurmaAsync(Guid turmaId);
    Task ValidarRegrasMatriculaAsync(Turma turma, Guid alunoId);
    Task<IEnumerable<Turma>> ObterTurmasCompativeisParaTransferenciaAsync(Guid alunoId, Guid escolaId);
    Task<bool> ValidarCompatibilidadeSerieIdadeAsync(Serie serie, DateTime dataNascimentoAluno);
}