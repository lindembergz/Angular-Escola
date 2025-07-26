using SistemaGestaoEscolar.Academico.Dominio.ObjetosDeValor;
using SistemaGestaoEscolar.Academico.Dominio.Eventos;
using SistemaGestaoEscolar.Shared.Domain.Entities;

namespace SistemaGestaoEscolar.Academico.Dominio.Entidades;

public class Disciplina : AggregateRoot
{
    public string Nome { get; private set; }
    public string Codigo { get; private set; }
    public int CargaHoraria { get; private set; }
    public Serie Serie { get; private set; }
    public bool Obrigatoria { get; private set; }
    public string? Descricao { get; private set; }
    public Guid EscolaId { get; private set; }
    public bool Ativa { get; private set; }

    private readonly List<Guid> _preRequisitos = new();
    public IReadOnlyList<Guid> PreRequisitos => _preRequisitos.AsReadOnly();

    private Disciplina() { } // EF Constructor

    private Disciplina(string nome, string codigo, int cargaHoraria, Serie serie, 
                      bool obrigatoria, Guid escolaId, string? descricao = null)
    {
        Nome = nome;
        Codigo = codigo;
        CargaHoraria = cargaHoraria;
        Serie = serie;
        Obrigatoria = obrigatoria;
        EscolaId = escolaId;
        Descricao = descricao;
        Ativa = true;
    }

    public static Disciplina Criar(string nome, string codigo, int cargaHoraria, 
                                  Serie serie, bool obrigatoria, Guid escolaId, 
                                  string? descricao = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da disciplina é obrigatório");

        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("Código da disciplina é obrigatório");

        if (cargaHoraria <= 0)
            throw new ArgumentException("Carga horária deve ser maior que zero");

        if (nome.Length > 100)
            throw new ArgumentException("Nome da disciplina não pode ter mais de 100 caracteres");

        if (codigo.Length > 20)
            throw new ArgumentException("Código da disciplina não pode ter mais de 20 caracteres");

        var disciplina = new Disciplina(nome.Trim(), codigo.Trim().ToUpper(), 
                                       cargaHoraria, serie, obrigatoria, escolaId, 
                                       descricao?.Trim());

        disciplina.AddDomainEvent(new DisciplinaCriadaEvento(disciplina.Id, nome, codigo));
        
        return disciplina;
    }

    public void AtualizarInformacoes(string nome, int cargaHoraria, bool obrigatoria, 
                                    string? descricao = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da disciplina é obrigatório");

        if (cargaHoraria <= 0)
            throw new ArgumentException("Carga horária deve ser maior que zero");

        if (nome.Length > 100)
            throw new ArgumentException("Nome da disciplina não pode ter mais de 100 caracteres");

        Nome = nome.Trim();
        CargaHoraria = cargaHoraria;
        Obrigatoria = obrigatoria;
        Descricao = descricao?.Trim();

        AddDomainEvent(new DisciplinaAtualizadaEvento(Id, Nome));
    }

    public void AdicionarPreRequisito(Guid disciplinaPreRequisitoId)
    {
        if (disciplinaPreRequisitoId == Id)
            throw new InvalidOperationException("Disciplina não pode ser pré-requisito de si mesma");

        if (_preRequisitos.Contains(disciplinaPreRequisitoId))
            throw new InvalidOperationException("Pré-requisito já foi adicionado");

        _preRequisitos.Add(disciplinaPreRequisitoId);
        AddDomainEvent(new PreRequisitoAdicionadoEvento(Id, disciplinaPreRequisitoId));
    }

    public void RemoverPreRequisito(Guid disciplinaPreRequisitoId)
    {
        if (!_preRequisitos.Contains(disciplinaPreRequisitoId))
            throw new InvalidOperationException("Pré-requisito não existe");

        _preRequisitos.Remove(disciplinaPreRequisitoId);
        AddDomainEvent(new PreRequisitoRemovidoEvento(Id, disciplinaPreRequisitoId));
    }

    public void Inativar()
    {
        if (!Ativa)
            throw new InvalidOperationException("Disciplina já está inativa");

        Ativa = false;
        AddDomainEvent(new DisciplinaInativadaEvento(Id, Nome));
    }

    public void Reativar()
    {
        if (Ativa)
            throw new InvalidOperationException("Disciplina já está ativa");

        Ativa = true;
        AddDomainEvent(new DisciplinaReativadaEvento(Id, Nome));
    }

    public bool PossuiPreRequisitos() => _preRequisitos.Any();
}