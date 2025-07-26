using SistemaGestaoEscolar.Academico.Dominio.Entidades;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Academico.Dominio.Repositorios;

namespace SistemaGestaoEscolar.Academico.Dominio.Servicos;

public class ServicosDominioTurma : IServicosDominioTurma
{
    private readonly IRepositorioTurma _repositorioTurma;

    public ServicosDominioTurma(IRepositorioTurma repositorioTurma)
    {
        _repositorioTurma = repositorioTurma;
    }

    public async Task<bool> PodeMatricularAlunoAsync(Guid turmaId, Guid alunoId)
    {
        var turma = await _repositorioTurma.ObterPorIdAsync(turmaId);
        if (turma == null || !turma.Ativa)
            return false;

        if (turma.AlunosMatriculados.Contains(alunoId))
            return false;

        return turma.PossuiVagasDisponiveis();
    }

    public async Task<bool> PodeCriarTurmaAsync(string nome, int anoLetivo, Guid escolaId)
    {
        return !await _repositorioTurma.ExisteNomeNaEscolaAsync(nome, escolaId);
    }

    public async Task<int> CalcularCapacidadeSugeridaAsync(Serie serie, TipoTurno tipoTurno)
    {
        return await Task.FromResult(serie.Tipo switch
        {
            TipoSerie.Infantil => tipoTurno == TipoTurno.Integral ? 15 : 20,
            TipoSerie.Fundamental when serie.Ano <= 5 => 25,
            TipoSerie.Fundamental => 30,
            TipoSerie.Medio => 35,
            _ => 30
        });
    }

    public async Task<bool> PodeInativarTurmaAsync(Guid turmaId)
    {
        var turma = await _repositorioTurma.ObterPorIdAsync(turmaId);
        if (turma == null || !turma.Ativa)
            return false;

        // Pode inativar se não há alunos matriculados ou se é final do ano letivo
        var possuiAlunos = turma.AlunosMatriculados.Any();
        var fimAnoLetivo = DateTime.Now.Month >= 11; // Novembro/Dezembro

        return !possuiAlunos || fimAnoLetivo;
    }

    public async Task ValidarRegrasMatriculaAsync(Turma turma, Guid alunoId)
    {
        if (!turma.Ativa)
            throw new InvalidOperationException("Não é possível matricular em turma inativa");

        if (turma.AlunosMatriculados.Contains(alunoId))
            throw new InvalidOperationException("Aluno já está matriculado nesta turma");

        if (!turma.PossuiVagasDisponiveis())
            throw new InvalidOperationException("Turma não possui vagas disponíveis");

        await Task.CompletedTask;
    }

    public async Task<IEnumerable<Turma>> ObterTurmasCompativeisParaTransferenciaAsync(Guid alunoId, Guid escolaId)
    {
        var turmasAtivas = await _repositorioTurma.ObterAtivasAsync();
        
        return turmasAtivas.Where(t => 
            t.EscolaId == escolaId &&
            !t.AlunosMatriculados.Contains(alunoId) && 
            t.PossuiVagasDisponiveis());
    }

    public async Task<bool> ValidarCompatibilidadeSerieIdadeAsync(Serie serie, DateTime dataNascimentoAluno)
    {
        var idade = DateTime.Now.Year - dataNascimentoAluno.Year;
        if (dataNascimentoAluno.Date > DateTime.Now.AddYears(-idade))
            idade--;

        var idadeMinima = serie.Tipo switch
        {
            TipoSerie.Infantil => 2 + (serie.Ano - 1),
            TipoSerie.Fundamental => 5 + serie.Ano,
            TipoSerie.Medio => 14 + serie.Ano,
            _ => 0
        };

        var idadeMaxima = idadeMinima + 3; // Tolerância de 3 anos

        return await Task.FromResult(idade >= idadeMinima && idade <= idadeMaxima);
    }
}