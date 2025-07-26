using SistemaGestaoEscolar.Professores.Dominio.Entidades;
using SistemaGestaoEscolar.Professores.Dominio.Repositorios;
using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Professores.Dominio.Servicos;

public class ServicosDominioProfessor : IServicosDominioProfessor
{
    private readonly IRepositorioProfessor _repositorioProfessor;

    public ServicosDominioProfessor(IRepositorioProfessor repositorioProfessor)
    {
        _repositorioProfessor = repositorioProfessor ?? throw new ArgumentNullException(nameof(repositorioProfessor));
    }

    public async Task ValidarCpfUnicoAsync(Cpf cpf, Guid? professorIdExcluir = null)
    {
        var professorExistente = await _repositorioProfessor.ObterPorCpfAsync(cpf);
        
        if (professorExistente != null && professorExistente.Id != professorIdExcluir)
        {
            throw new InvalidOperationException($"Já existe um professor cadastrado com o CPF {cpf.NumeroFormatado}");
        }
    }

    public async Task ValidarRegistroUnicoAsync(string registro, Guid? professorIdExcluir = null)
    {
        var professorExistente = await _repositorioProfessor.ObterPorRegistroAsync(registro);
        
        if (professorExistente != null && professorExistente.Id != professorIdExcluir)
        {
            throw new InvalidOperationException($"Já existe um professor cadastrado com o registro {registro}");
        }
    }

    public async Task<bool> PodeAtribuirDisciplinaAsync(Guid professorId, Guid disciplinaId)
    {
        var professor = await _repositorioProfessor.ObterPorIdAsync(professorId);
        if (professor == null || !professor.Ativo)
            return false;

        // Verificar se já leciona a disciplina
        if (professor.Disciplinas.Any(d => d.DisciplinaId == disciplinaId))
            return false;

        // Verificar se tem carga horária disponível (máximo 40h semanais)
        var cargaHorariaAtual = professor.ObterCargaHorariaTotal();
        return cargaHorariaAtual < 40;
    }

    public async Task<int> CalcularCargaHorariaTotalAsync(Guid professorId)
    {
        var professor = await _repositorioProfessor.ObterPorIdAsync(professorId);
        return professor?.ObterCargaHorariaTotal() ?? 0;
    }

    public async Task<bool> ProfessorPodeSerDesativadoAsync(Guid professorId)
    {
        var professor = await _repositorioProfessor.ObterPorIdAsync(professorId);
        if (professor == null || !professor.Ativo)
            return false;

        // Verificar se tem disciplinas ativas
        // Em um cenário real, verificaríamos também se tem turmas ativas, avaliações pendentes, etc.
        return !professor.Disciplinas.Any(d => d.Ativa);
    }

    public async Task<IEnumerable<Professor>> ObterProfessoresDisponiveisParaDisciplinaAsync(Guid disciplinaId, Guid escolaId)
    {
        var professoresDaEscola = await _repositorioProfessor.ObterPorEscolaAsync(escolaId);
        
        return professoresDaEscola.Where(p => 
            p.Ativo && 
            !p.Disciplinas.Any(d => d.DisciplinaId == disciplinaId) && 
            p.ObterCargaHorariaTotal() < 40);
    }

    public async Task<bool> ValidarCapacidadeEscolaAsync(Guid escolaId)
    {
        var quantidadeProfessores = await _repositorioProfessor.ContarPorEscolaAsync(escolaId);
        
        // Limite arbitrário de 200 professores por escola
        // Em um cenário real, isso poderia vir de configurações da escola
        return quantidadeProfessores < 200;
    }
}