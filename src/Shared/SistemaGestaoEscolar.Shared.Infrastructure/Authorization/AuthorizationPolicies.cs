namespace SistemaGestaoEscolar.Shared.Infrastructure.Authorization;

/// <summary>
/// Constantes para todas as políticas de autorização do sistema.
/// Centraliza a definição de políticas para facilitar manutenção e consistência.
/// </summary>
public static class AuthorizationPolicies
{
    /// <summary>
    /// Política para Super Administradores - acesso total ao sistema
    /// </summary>
    public const string SuperAdmin = "SuperAdmin";

    /// <summary>
    /// Política para Administradores - acesso administrativo geral
    /// </summary>
    public const string Admin = "Admin";

    /// <summary>
    /// Política para equipe escolar - acesso a funcionalidades da escola
    /// </summary>
    public const string SchoolStaff = "SchoolStaff";

    /// <summary>
    /// Política para professores - acesso a funcionalidades acadêmicas
    /// </summary>
    public const string Teacher = "Teacher";

    /// <summary>
    /// Política para secretários - acesso a funcionalidades administrativas da escola
    /// </summary>
    public const string Secretary = "Secretary";

    /// <summary>
    /// Política para diretores - acesso de gestão da escola
    /// </summary>
    public const string Director = "Director";

    /// <summary>
    /// Política para coordenadores - acesso de coordenação acadêmica
    /// </summary>
    public const string Coordinator = "Coordinator";

    /// <summary>
    /// Política para acesso acadêmico - leitura de dados acadêmicos
    /// </summary>
    public const string AcademicAccess = "AcademicAccess";

    /// <summary>
    /// Política para acesso financeiro - leitura de dados financeiros
    /// </summary>
    public const string FinancialAccess = "FinancialAccess";

    /// <summary>
    /// Política para geração de relatórios
    /// </summary>
    public const string ReportsAccess = "ReportsAccess";

    /// <summary>
    /// Política para configuração do sistema
    /// </summary>
    public const string SystemConfig = "SystemConfig";

    /// <summary>
    /// Política para usuários com email confirmado
    /// </summary>
    public const string EmailConfirmed = "EmailConfirmed";

    /// <summary>
    /// Política para acesso específico à escola do usuário
    /// </summary>
    public const string SchoolAccess = "SchoolAccess";

    /// <summary>
    /// Política para gerenciamento de usuários
    /// </summary>
    public const string UserManagement = "UserManagement";

    /// <summary>
    /// Política para gerenciamento de alunos
    /// </summary>
    public const string StudentManagement = "StudentManagement";

    /// <summary>
    /// Política para gerenciamento de escolas
    /// </summary>
    public const string SchoolManagement = "SchoolManagement";

    /// <summary>
    /// Política para gerenciamento de redes escolares
    /// </summary>
    public const string NetworkManagement = "NetworkManagement";

    /// <summary>
    /// Política para gerenciamento de professores
    /// </summary>
    public const string TeacherManagement = "TeacherManagement";
}