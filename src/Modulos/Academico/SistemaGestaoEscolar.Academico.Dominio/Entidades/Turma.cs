using SistemaGestaoEscolar.Shared.Domain.Entities;
using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Academico.Dominio.Eventos;

namespace SistemaGestaoEscolar.Academico.Dominio.Entidades;

public class Turma : AggregateRoot
{
    public NomeTurma Nome { get; private set; }
    public Serie Serie { get; private set; }
    public Turno Turno { get; private set; }
    public int CapacidadeMaxima { get; private set; }
    public int AnoLetivo { get; private set; }
    public Guid EscolaId { get; private set; }
    public bool Ativa { get; private set; }

    private readonly List<Guid> _alunosMatriculados = new();
    public IReadOnlyList<Guid> AlunosMatriculados => _alunosMatriculados.AsReadOnly();

    private readonly List<Disciplina> _disciplinas = new();
    public IReadOnlyList<Disciplina> Disciplinas => _disciplinas.AsReadOnly();

    private Turma() { } // EF Constructor

    private Turma(NomeTurma nome, Serie serie, Turno turno, int capacidadeMaxima, 
                  int anoLetivo, Guid escolaId)
    {
        Nome = nome;
        Serie = serie;
        Turno = turno;
        CapacidadeMaxima = capacidadeMaxima;
        AnoLetivo = anoLetivo;
        EscolaId = escolaId;
        Ativa = true;
    }

    public static Turma Criar(NomeTurma nome, Serie serie, Turno turno, 
                             int capacidadeMaxima, int anoLetivo, Guid escolaId)
    {
        if (capacidadeMaxima <= 0)
            throw new ArgumentException("Capacidade máxima deve ser maior que zero");

        if (anoLetivo < DateTime.Now.Year - 1)
            throw new ArgumentException("Ano letivo não pode ser anterior ao ano passado");

        var turma = new Turma(nome, serie, turno, capacidadeMaxima, anoLetivo, escolaId);
        turma.AddDomainEvent(new TurmaCriadaEvento(turma.Id, nome.Valor, serie.Descricao));
        
        return turma;
    }

    public void MatricularAluno(Guid alunoId)
    {
        if (_alunosMatriculados.Contains(alunoId))
            throw new InvalidOperationException("Aluno já está matriculado nesta turma");

        if (_alunosMatriculados.Count >= CapacidadeMaxima)
            throw new InvalidOperationException("Turma já atingiu capacidade máxima");

        if (!Ativa)
            throw new InvalidOperationException("Não é possível matricular aluno em turma inativa");

        _alunosMatriculados.Add(alunoId);
        AddDomainEvent(new TurmaInativadaEvento(Id, Nome.Valor));
    }

    public void RemoverAluno(Guid alunoId)
    {
        if (!_alunosMatriculados.Contains(alunoId))
            throw new InvalidOperationException("Aluno não está matriculado nesta turma");

        _alunosMatriculados.Remove(alunoId);
        AddDomainEvent(new AlunoRemovidoDaTurmaEvento(Id, alunoId));
    }

    public void AdicionarDisciplina(Disciplina disciplina)
    {
        if (_disciplinas.Any(d => d.Id == disciplina.Id))
            throw new InvalidOperationException("Disciplina já está associada à turma");

        _disciplinas.Add(disciplina);
    }

    public void RemoverDisciplina(Guid disciplinaId)
    {
        var disciplina = _disciplinas.FirstOrDefault(d => d.Id == disciplinaId);
        if (disciplina == null)
            throw new InvalidOperationException("Disciplina não está associada à turma");

        _disciplinas.Remove(disciplina);
    }

    public void Inativar()
    {
        if (!Ativa)
            throw new InvalidOperationException("Turma já está inativa");

        Ativa = false;
        AddDomainEvent(new TurmaInativadaEvento(Id, Nome.Valor));
    }

    public void Reativar()
    {
        if (Ativa)
            throw new InvalidOperationException("Turma já está ativa");

        Ativa = true;
        AddDomainEvent(new TurmaReativadaEvento(Id, Nome.Valor));
    }

    public int ObterVagasDisponiveis() => CapacidadeMaxima - _alunosMatriculados.Count;

    public bool PossuiVagasDisponiveis() => ObterVagasDisponiveis() > 0;
}