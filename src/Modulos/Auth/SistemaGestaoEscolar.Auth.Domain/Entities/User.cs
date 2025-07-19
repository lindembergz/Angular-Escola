using SistemaGestaoEscolar.Auth.Domain.ValueObjects;
using SistemaGestaoEscolar.Auth.Domain.Events;
using SistemaGestaoEscolar.Shared.Domain.Entities;

namespace SistemaGestaoEscolar.Auth.Domain.Entities;

/// <summary>
/// Entidade que representa um usuário do sistema.
/// Aggregate Root responsável por gerenciar autenticação e autorização.
/// </summary>
public class User : AggregateRoot
{
    public string PrimeiroNome { get; private set; } = string.Empty;
    public string UltimoNome { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public Password Senha { get; private set; } = null!;
    public UserRole Perfil { get; private set; } = null!;
    public Guid? EscolaId { get; private set; }
    public bool Ativo { get; private set; }
    public bool EmailConfirmado { get; private set; }
    public DateTime? UltimoLoginEm { get; private set; }
    public DateTime? UltimaMudancaSenhaEm { get; private set; }
    public int TentativasLoginFalhadas { get; private set; }
    public DateTime? BloqueadoAte { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiraEm { get; private set; }

    private readonly List<Session> _sessions = new();
    public IReadOnlyCollection<Session> Sessions => _sessions.AsReadOnly();

    private User() { } // Para EF Core

    public User(
        string primeiroNome,
        string ultimoNome,
        Email email,
        Password senha,
        UserRole perfil,
        Guid? escolaId = null)
    {
        if (string.IsNullOrWhiteSpace(primeiroNome))
            throw new ArgumentException("Nome não pode ser vazio", nameof(primeiroNome));

        if (string.IsNullOrWhiteSpace(ultimoNome))
            throw new ArgumentException("Sobrenome não pode ser vazio", nameof(ultimoNome));

        PrimeiroNome = primeiroNome.Trim();
        UltimoNome = ultimoNome.Trim();
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Senha = senha ?? throw new ArgumentNullException(nameof(senha));
        Perfil = perfil ?? throw new ArgumentNullException(nameof(perfil));
        EscolaId = escolaId;
        Ativo = true;
        EmailConfirmado = false;
        UltimaMudancaSenhaEm = DateTime.UtcNow;
        TentativasLoginFalhadas = 0;

        AddDomainEvent(new UserCreatedEvent(Id, Email.Value, Perfil.Code));
    }

    /// <summary>
    /// Nome completo do usuário
    /// </summary>
    public string NomeCompleto => $"{PrimeiroNome} {UltimoNome}".Trim();

    /// <summary>
    /// Iniciais do usuário
    /// </summary>
    public string Iniciais => $"{PrimeiroNome.FirstOrDefault()}{UltimoNome.FirstOrDefault()}".ToUpperInvariant();

    /// <summary>
    /// Verifica se a senha fornecida está correta
    /// </summary>
    public bool VerificarSenha(string senhaTextoPlano)
    {
        if (string.IsNullOrWhiteSpace(senhaTextoPlano))
            return false;

        return Senha.Verificar(senhaTextoPlano);
    }

    /// <summary>
    /// Altera a senha do usuário
    /// </summary>
    public void AlterarSenha(Password novaSenha)
    {
        if (novaSenha == null)
            throw new ArgumentNullException(nameof(novaSenha));

        Senha = novaSenha;
        UltimaMudancaSenhaEm = DateTime.UtcNow;
        RefreshToken = null;
        RefreshTokenExpiraEm = null;

        // Invalidar todas as sessões ativas
        foreach (var session in _sessions.Where(s => s.Ativa))
        {
            session.Invalidar();
        }

        AddDomainEvent(new UserPasswordChangedEvent(Id, Email.Value));
        MarkAsUpdated();
    }

    /// <summary>
    /// Registra uma tentativa de login bem-sucedida
    /// </summary>
    public void RegistrarLoginBemSucedido()
    {
        UltimoLoginEm = DateTime.UtcNow;
        TentativasLoginFalhadas = 0;
        BloqueadoAte = null;

        AddDomainEvent(new UserLoggedInEvent(Id, Email.Value, DateTime.UtcNow));
        MarkAsUpdated();
    }

    /// <summary>
    /// Registra uma tentativa de login falhada
    /// </summary>
    public void RegistrarLoginFalhado()
    {
        TentativasLoginFalhadas++;

        // Bloquear conta após 5 tentativas falhadas
        if (TentativasLoginFalhadas >= 5)
        {
            BloqueadoAte = DateTime.UtcNow.AddMinutes(30); // Bloquear por 30 minutos
            AddDomainEvent(new UserAccountLockedEvent(Id, Email.Value, BloqueadoAte.Value));
        }

        AddDomainEvent(new UserLoginFailedEvent(Id, Email.Value, TentativasLoginFalhadas));
        MarkAsUpdated();
    }

    /// <summary>
    /// Verifica se a conta está bloqueada
    /// </summary>
    public bool EstaBloqueada()
    {
        return BloqueadoAte.HasValue && BloqueadoAte.Value > DateTime.UtcNow;
    }

    /// <summary>
    /// Desbloqueia a conta manualmente
    /// </summary>
    public void Desbloquear()
    {
        TentativasLoginFalhadas = 0;
        BloqueadoAte = null;

        AddDomainEvent(new UserAccountUnlockedEvent(Id, Email.Value));
        MarkAsUpdated();
    }

    /// <summary>
    /// Ativa o usuário
    /// </summary>
    public void Ativar()
    {
        if (Ativo) return;

        Ativo = true;
        AddDomainEvent(new UserActivatedEvent(Id, Email.Value));
        MarkAsUpdated();
    }

    /// <summary>
    /// Desativa o usuário
    /// </summary>
    public void Desativar()
    {
        if (!Ativo) return;

        Ativo = false;
        RefreshToken = null;
        RefreshTokenExpiraEm = null;

        // Invalidar todas as sessões ativas
        foreach (var session in _sessions.Where(s => s.Ativa) )
        {
            session.Invalidar();
        }

        AddDomainEvent(new UserDeactivatedEvent(Id, Email.Value));
        MarkAsUpdated();
    }

    /// <summary>
    /// Confirma o email do usuário
    /// </summary>
    public void ConfirmarEmail()
    {
        if (EmailConfirmado) return;

        EmailConfirmado = true;
        AddDomainEvent(new UserEmailConfirmedEvent(Id, Email.Value));
        MarkAsUpdated();
    }

    /// <summary>
    /// Atualiza o refresh token
    /// </summary>
    public void DefinirRefreshToken(string refreshToken, DateTime expiraEm)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token não pode ser vazio", nameof(refreshToken));

        if (expiraEm <= DateTime.UtcNow)
            throw new ArgumentException("Data de expiração deve ser futura", nameof(expiraEm));

        RefreshToken = refreshToken;
        RefreshTokenExpiraEm = expiraEm;
        MarkAsUpdated();
    }

    /// <summary>
    /// Remove o refresh token
    /// </summary>
    public void LimparRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiraEm = null;
        MarkAsUpdated();
    }

    /// <summary>
    /// Verifica se o refresh token é válido
    /// </summary>
    public bool RefreshTokenEhValido(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken) || string.IsNullOrWhiteSpace(RefreshToken))
            return false;

        return RefreshToken == refreshToken && 
               RefreshTokenExpiraEm.HasValue && 
               RefreshTokenExpiraEm.Value > DateTime.UtcNow;
    }

    /// <summary>
    /// Cria uma nova sessão para o usuário
    /// </summary>
    public Session CriarSessao(string enderecoIp, string agenteUsuario)
    {
        var session = new Session(Id, enderecoIp, agenteUsuario);
        _sessions.Add(session);
        return session;
    }

    /// <summary>
    /// Invalida todas as sessões ativas
    /// </summary>
    public void InvalidarTodasSessoes()
    {
        foreach (var session in _sessions.Where(s => s.Ativa))
        {
            session.Invalidar();
        }

        AddDomainEvent(new UserAllSessionsInvalidatedEvent(Id, Email.Value));
        MarkAsUpdated();
    }

    /// <summary>
    /// Atualiza informações básicas do usuário
    /// </summary>
    public void AtualizarInformacoesBasicas(string primeiroNome, string ultimoNome)
    {
        if (string.IsNullOrWhiteSpace(primeiroNome))
            throw new ArgumentException("Nome não pode ser vazio", nameof(primeiroNome));

        if (string.IsNullOrWhiteSpace(ultimoNome))
            throw new ArgumentException("Sobrenome não pode ser vazio", nameof(ultimoNome));

        var nomeCompletoAntigo = NomeCompleto;
        PrimeiroNome = primeiroNome.Trim();
        UltimoNome = ultimoNome.Trim();

        AddDomainEvent(new UserInfoUpdatedEvent(Id, Email.Value, nomeCompletoAntigo, NomeCompleto));
        MarkAsUpdated();
    }

    /// <summary>
    /// Atualiza o papel do usuário
    /// </summary>
    public void AtualizarPerfil(UserRole novoPerfil)
    {
        if (novoPerfil == null)
            throw new ArgumentNullException(nameof(novoPerfil));

        if (Perfil == novoPerfil) return;

        var perfilAntigo = Perfil;
        Perfil = novoPerfil;

        // Invalidar todas as sessões quando o papel muda
        InvalidarTodasSessoes();

        AddDomainEvent(new UserRoleChangedEvent(Id, Email.Value, perfilAntigo.Code, novoPerfil.Code));
        MarkAsUpdated();
    }

    /// <summary>
    /// Verifica se o usuário pode acessar uma escola específica
    /// </summary>
    public bool PodeAcessarEscola(Guid escolaId)
    {
        // SuperAdmin pode acessar qualquer escola
        if (Perfil == UserRole.SuperAdmin)
            return true;

        // Usuários sem escola associada não podem acessar nenhuma escola específica
        if (!EscolaId.HasValue)
            return false;

        // Usuários só podem acessar sua própria escola
        return EscolaId.Value == escolaId;
    }

    /// <summary>
    /// Verifica se a senha precisa ser alterada (política de expiração)
    /// </summary>
    public bool SenhaPrecisaSerAlterada(int maxDiasSemMudanca = 90)
    {
        if (!UltimaMudancaSenhaEm.HasValue)
            return true;

        return DateTime.UtcNow.Subtract(UltimaMudancaSenhaEm.Value).TotalDays > maxDiasSemMudanca;
    }

    /// <summary>
    /// Verifica se o usuário está inativo há muito tempo
    /// </summary>
    public bool EstaInativo(int maxDiasSemLogin = 180)
    {
        if (!UltimoLoginEm.HasValue)
            return DateTime.UtcNow.Subtract(CreatedAt).TotalDays > maxDiasSemLogin;

        return DateTime.UtcNow.Subtract(UltimoLoginEm.Value).TotalDays > maxDiasSemLogin;
    }
}