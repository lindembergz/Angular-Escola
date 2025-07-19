using SistemaGestaoEscolar.Shared.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Auth.Domain.ValueObjects;

/// <summary>
/// Value Object que representa o papel/perfil de um usuário no sistema.
/// Define as permissões e responsabilidades do usuário.
/// </summary>
public class UserRole : ValueObject
{
    public static readonly UserRole SuperAdmin = new("SuperAdmin", "Super Administrador", 1000);
    public static readonly UserRole Admin = new("Admin", "Administrador", 900);
    public static readonly UserRole Director = new("Director", "Diretor", 800);
    public static readonly UserRole Coordinator = new("Coordinator", "Coordenador Pedagógico", 700);
    public static readonly UserRole Secretary = new("Secretary", "Secretário", 600);
    public static readonly UserRole FinancialManager = new("FinancialManager", "Responsável Financeiro", 500);
    public static readonly UserRole Teacher = new("Teacher", "Professor", 400);
    public static readonly UserRole Parent = new("Parent", "Responsável", 300);
    public static readonly UserRole Student = new("Student", "Aluno", 200);
    public static readonly UserRole Guest = new("Guest", "Convidado", 100);

    private static readonly Dictionary<string, UserRole> AllRoles = new()
    {
        { SuperAdmin.Code, SuperAdmin },
        { Admin.Code, Admin },
        { Director.Code, Director },
        { Coordinator.Code, Coordinator },
        { Secretary.Code, Secretary },
        { FinancialManager.Code, FinancialManager },
        { Teacher.Code, Teacher },
        { Parent.Code, Parent },
        { Student.Code, Student },
        { Guest.Code, Guest }
    };

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int Level { get; private set; }

    private UserRole() { } // Para EF Core

    private UserRole(string code, string name, int level)
    {
        Code = code;
        Name = name;
        Level = level;
    }

    public static UserRole FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Código do papel não pode ser vazio", nameof(code));

        if (!AllRoles.TryGetValue(code, out var role))
            throw new ArgumentException($"Papel '{code}' não é válido", nameof(code));

        return role;
    }

    public static UserRole[] GetAllRoles() => AllRoles.Values.ToArray();

    public static UserRole[] GetRolesForSchoolStaff() => new[]
    {
        Admin, Director, Coordinator, Secretary, FinancialManager, Teacher
    };

    public static UserRole[] GetRolesForExternalUsers() => new[]
    {
        Parent, Student
    };

    /// <summary>
    /// Verifica se este papel tem autoridade sobre outro papel
    /// </summary>
    public bool TemAutoridadeSobre(UserRole outroPerfil)
    {
        if (outroPerfil == null)
            return true;

        return Level > outroPerfil.Level;
    }

    /// <summary>
    /// Verifica se este papel pode gerenciar usuários com outro papel
    /// </summary>
    public bool PodeGerenciar(UserRole outroPerfil)
    {
        if (outroPerfil == null)
            return false;

        // SuperAdmin pode gerenciar todos
        if (this == SuperAdmin)
            return true;

        // Admin pode gerenciar todos exceto SuperAdmin
        if (this == Admin)
            return outroPerfil != SuperAdmin;

        // Diretor pode gerenciar coordenadores, secretários, professores, pais e alunos
        if (this == Director)
            return outroPerfil.Level <= Coordinator.Level && outroPerfil != SuperAdmin && outroPerfil != Admin;

        // Coordenador pode gerenciar professores, pais e alunos
        if (this == Coordinator)
            return outroPerfil.Level <= Teacher.Level && outroPerfil != SuperAdmin && outroPerfil != Admin && outroPerfil != Director;

        // Secretário pode gerenciar pais e alunos
        if (this == Secretary)
            return outroPerfil == Parent || outroPerfil == Student;

        return false;
    }

    /// <summary>
    /// Verifica se este papel tem permissão para acessar dados acadêmicos
    /// </summary>
    public bool PodeAcessarDadosAcademicos()
    {
        return this == SuperAdmin || this == Admin || this == Director || 
               this == Coordinator || this == Secretary || this == Teacher;
    }

    /// <summary>
    /// Verifica se este papel tem permissão para acessar dados financeiros
    /// </summary>
    public bool PodeAcessarDadosFinanceiros()
    {
        return this == SuperAdmin || this == Admin || this == Director || 
               this == FinancialManager;
    }

    /// <summary>
    /// Verifica se este papel pode gerar relatórios
    /// </summary>
    public bool PodeGerarRelatorios()
    {
        return this == SuperAdmin || this == Admin || this == Director || 
               this == Coordinator || this == FinancialManager;
    }

    /// <summary>
    /// Verifica se este papel pode configurar o sistema
    /// </summary>
    public bool PodeConfigurarSistema()
    {
        return this == SuperAdmin || this == Admin;
    }

    /// <summary>
    /// Verifica se este papel representa um funcionário da escola
    /// </summary>
    public bool EhFuncionarioEscola()
    {
        return Level >= Teacher.Level && this != Parent && this != Student && this != Guest;
    }

    /// <summary>
    /// Verifica se este papel representa um usuário externo
    /// </summary>
    public bool EhUsuarioExterno()
    {
        return this == Parent || this == Student || this == Guest;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Name;

    public static implicit operator string(UserRole role) => role?.Code ?? string.Empty;
}