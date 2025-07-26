namespace SistemaGestaoEscolar.Professores.Dominio.DTOs;

/// <summary>
/// DTO com informações básicas de uma disciplina para integração entre módulos
/// </summary>
public record DisciplinaInfoDto(
    Guid Id,
    string Nome,
    string Codigo,
    int CargaHoraria,
    string Serie,
    bool Obrigatoria,
    bool Ativa,
    Guid EscolaId);