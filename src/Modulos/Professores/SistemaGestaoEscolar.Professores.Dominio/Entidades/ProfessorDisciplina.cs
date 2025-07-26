using SistemaGestaoEscolar.Shared.Domain.Entities;

namespace SistemaGestaoEscolar.Professores.Dominio.Entidades;

public class ProfessorDisciplina : BaseEntity
{
    public Guid ProfessorId { get; private set; }
    public Guid DisciplinaId { get; private set; }
    public int CargaHorariaSemanal { get; private set; }
    public DateTime DataAtribuicao { get; private set; }
    public bool Ativa { get; private set; }
    public string? Observacoes { get; private set; }

    private ProfessorDisciplina() { } // Para EF Core

    public ProfessorDisciplina(Guid professorId, Guid disciplinaId, int cargaHorariaSemanal, string? observacoes = null)
    {
        if (professorId == Guid.Empty)
            throw new ArgumentException("ProfessorId não pode ser vazio");

        if (disciplinaId == Guid.Empty)
            throw new ArgumentException("DisciplinaId não pode ser vazio");

        if (cargaHorariaSemanal <= 0 || cargaHorariaSemanal > 40)
            throw new ArgumentException("Carga horária semanal deve estar entre 1 e 40 horas");

        Id = Guid.NewGuid();
        ProfessorId = professorId;
        DisciplinaId = disciplinaId;
        CargaHorariaSemanal = cargaHorariaSemanal;
        DataAtribuicao = DateTime.UtcNow;
        Ativa = true;
        Observacoes = string.IsNullOrWhiteSpace(observacoes) ? null : observacoes.Trim();
    }

    public void AtualizarCargaHoraria(int novaCargaHoraria)
    {
        if (novaCargaHoraria <= 0 || novaCargaHoraria > 40)
            throw new ArgumentException("Carga horária semanal deve estar entre 1 e 40 horas");

        CargaHorariaSemanal = novaCargaHoraria;
    }

    public void AtualizarObservacoes(string? novasObservacoes)
    {
        Observacoes = string.IsNullOrWhiteSpace(novasObservacoes) ? null : novasObservacoes.Trim();
    }

    public void Desativar()
    {
        Ativa = false;
    }

    public void Ativar()
    {
        Ativa = true;
    }

    public int ObterDiasAtribuicao()
    {
        return (DateTime.UtcNow - DataAtribuicao).Days;
    }

    public bool EhAtribuicaoRecente()
    {
        return ObterDiasAtribuicao() <= 30;
    }
}