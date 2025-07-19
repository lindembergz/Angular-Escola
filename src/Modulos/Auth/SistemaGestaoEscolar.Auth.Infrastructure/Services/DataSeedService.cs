using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Domain.Entities;
using SistemaGestaoEscolar.Auth.Domain.Repositories;
using SistemaGestaoEscolar.Auth.Domain.ValueObjects;

namespace SistemaGestaoEscolar.Auth.Infrastructure.Services;

/// <summary>
/// Serviço para inicialização de dados básicos do sistema.
/// Cria usuários padrão e dados essenciais para funcionamento.
/// </summary>
public class DataSeedService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DataSeedService> _logger;

    public DataSeedService(
        IUserRepository userRepository,
        ILogger<DataSeedService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Inicializa dados básicos do sistema
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Iniciando seed de dados de autenticação");

            await CreateDefaultSuperAdminAsync();
            await CreateDefaultAdminAsync();
            await CreateSampleUsersAsync();

            _logger.LogInformation("Seed de dados de autenticação concluído");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante seed de dados de autenticação");
            throw;
        }
    }

    /// <summary>
    /// Cria o usuário SuperAdmin padrão
    /// </summary>
    private async Task CreateDefaultSuperAdminAsync()
    {
        const string superAdminEmail = "superadmin@sistemaescolar.com";
        
        var existingSuperAdmin = await _userRepository.ObterPorEmailAsync(superAdminEmail);
        if (existingSuperAdmin != null)
        {
            _logger.LogInformation("SuperAdmin já existe, pulando criação");
            return;
        }

        var email = new Email(superAdminEmail);
        var password = new Password("SuperAdmin@2024");

        var superAdmin = new User(
            "Super",
            "Administrador",
            email,
            password,
            UserRole.SuperAdmin
        );

        // Confirmar email automaticamente para usuário padrão
        superAdmin.ConfirmarEmail();

        await _userRepository.AdicionarAsync(superAdmin);
        await _userRepository.SalvarAlteracoesAsync();

        _logger.LogInformation("SuperAdmin criado com sucesso: {Email}", superAdminEmail);
    }

    /// <summary>
    /// Cria o usuário Admin padrão
    /// </summary>
    private async Task CreateDefaultAdminAsync()
    {
        const string adminEmail = "admin@sistemaescolar.com";
        
        var existingAdmin = await _userRepository.ObterPorEmailAsync(adminEmail);
        if (existingAdmin != null)
        {
            _logger.LogInformation("Admin já existe, pulando criação");
            return;
        }

        var email = new Email(adminEmail);
        var password = new Password("Admin@2024");

        var admin = new User(
            "Administrador",
            "Sistema",
            email,
            password,
            UserRole.Admin
        );

        // Confirmar email automaticamente para usuário padrão
        admin.ConfirmarEmail();

        await _userRepository.AdicionarAsync(admin);
        await _userRepository.SalvarAlteracoesAsync();

        _logger.LogInformation("Admin criado com sucesso: {Email}", adminEmail);
    }

    /// <summary>
    /// Cria usuários de exemplo para desenvolvimento
    /// </summary>
    private async Task CreateSampleUsersAsync()
    {
        var sampleUsers = new[]
        {
            new { Email = "diretor@escola.com", FirstName = "João", LastName = "Silva", Role = UserRole.Director },
            new { Email = "coordenador@escola.com", FirstName = "Maria", LastName = "Santos", Role = UserRole.Coordinator },
            new { Email = "secretario@escola.com", FirstName = "Ana", LastName = "Costa", Role = UserRole.Secretary },
            new { Email = "professor@escola.com", FirstName = "Carlos", LastName = "Oliveira", Role = UserRole.Teacher },
            new { Email = "responsavel@email.com", FirstName = "Pedro", LastName = "Souza", Role = UserRole.Parent },
        };

        foreach (var userData in sampleUsers)
        {
            var existingUser = await _userRepository.ObterPorEmailAsync(userData.Email);
            if (existingUser != null)
            {
                continue; // Usuário já existe
            }

            try
            {
                var email = new Email(userData.Email);
                var password = new Password("Senha@123");

                var user = new User(
                    userData.FirstName,
                    userData.LastName,
                    email,
                    password,
                    userData.Role
                );

                // Confirmar email automaticamente para usuários de exemplo
                user.ConfirmarEmail();

                await _userRepository.AdicionarAsync(user);
                
                _logger.LogInformation("Usuário de exemplo criado: {Email} ({Role})", 
                    userData.Email, userData.Role.Name);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao criar usuário de exemplo: {Email}", userData.Email);
            }
        }

        await _userRepository.SalvarAlteracoesAsync();
    }

    /// <summary>
    /// Verifica se o sistema precisa de seed inicial
    /// </summary>
    public async Task<bool> NeedsSeedAsync()
    {
        try
        {
            var statistics = await _userRepository.ObterEstatisticasAsync();
            return statistics.TotalUsuarios == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar necessidade de seed");
            return false;
        }
    }

    /// <summary>
    /// Cria um usuário específico (útil para testes)
    /// </summary>
    public async Task<User> CreateUserAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        UserRole role,
        Guid? schoolId = null,
        bool confirmEmail = false)
    {
        try
        {
            var emailObj = new Email(email);
            var passwordObj = new Password(password);

            var user = new User(firstName, lastName, emailObj, passwordObj, role, schoolId);

            if (confirmEmail)
            {
                user.ConfirmarEmail();
            }

            await _userRepository.AdicionarAsync(user);
            await _userRepository.SalvarAlteracoesAsync();

            _logger.LogInformation("Usuário criado via seed: {Email} ({Role})", email, role.Name);

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário via seed: {Email}", email);
            throw;
        }
    }
}