using SistemaGestaoEscolar.Shared.Domain.Entities;

namespace SistemaGestaoEscolar.Auth.Domain.Entities;

/// <summary>
/// Entidade que representa uma sessão de usuário ativa no sistema.
/// Controla o acesso e rastreia atividade do usuário.
/// </summary>
public class Session : BaseEntity
{
    public Guid UsuarioId { get; private set; }
    public string EnderecoIp { get; private set; } = string.Empty;
    public string AgenteUsuario { get; private set; } = string.Empty;
    public DateTime IniciadaEm { get; private set; }
    public DateTime UltimaAtividadeEm { get; private set; }
    public DateTime? FinalizadaEm { get; private set; }
    public bool Ativa { get; private set; }
    public string? InfoDispositivo { get; private set; }
    public string? Localizacao { get; private set; }

    private Session() { } // Para EF Core

    public Session(Guid usuarioId, string enderecoIp, string agenteUsuario)
    {
        if (usuarioId == Guid.Empty)
            throw new ArgumentException("ID do usuário não pode ser vazio", nameof(usuarioId));

        if (string.IsNullOrWhiteSpace(enderecoIp))
            throw new ArgumentException("Endereço IP não pode ser vazio", nameof(enderecoIp));

        if (string.IsNullOrWhiteSpace(agenteUsuario))
            throw new ArgumentException("User Agent não pode ser vazio", nameof(agenteUsuario));

        UsuarioId = usuarioId;
        EnderecoIp = enderecoIp.Trim();
        AgenteUsuario = agenteUsuario.Trim();
        IniciadaEm = DateTime.UtcNow;
        UltimaAtividadeEm = DateTime.UtcNow;
        Ativa = true;

        // Extrair informações do dispositivo do User Agent
        InfoDispositivo = ExtrairInfoDispositivo(agenteUsuario);
    }

    /// <summary>
    /// Duração da sessão
    /// </summary>
    public TimeSpan Duracao => (FinalizadaEm ?? DateTime.UtcNow) - IniciadaEm;

    /// <summary>
    /// Tempo desde a última atividade
    /// </summary>
    public TimeSpan TempoDesdeUltimaAtividade => DateTime.UtcNow - UltimaAtividadeEm;

    /// <summary>
    /// Atualiza a última atividade da sessão
    /// </summary>
    public void AtualizarAtividade()
    {
        if (!Ativa)
            throw new InvalidOperationException("Não é possível atualizar atividade de uma sessão inativa");

        UltimaAtividadeEm = DateTime.UtcNow;
        MarkAsUpdated();
    }

    /// <summary>
    /// Invalida a sessão
    /// </summary>
    public void Invalidar()
    {
        if (!Ativa) return;

        Ativa = false;
        FinalizadaEm = DateTime.UtcNow;
        MarkAsUpdated();
    }

    /// <summary>
    /// Verifica se a sessão expirou por inatividade
    /// </summary>
    public bool Expirou(int maxMinutosInativo = 30)
    {
        if (!Ativa) return true;

        return TempoDesdeUltimaAtividade.TotalMinutes > maxMinutosInativo;
    }

    /// <summary>
    /// Verifica se a sessão é de longa duração
    /// </summary>
    public bool EhLongaDuracao(int maxHoras = 8)
    {
        return Duracao.TotalHours > maxHoras;
    }

    /// <summary>
    /// Define informações de localização da sessão
    /// </summary>
    public void DefinirLocalizacao(string localizacao)
    {
        if (string.IsNullOrWhiteSpace(localizacao))
            return;

        Localizacao = localizacao.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Verifica se a sessão é suspeita (baseada em padrões de uso)
    /// </summary>
    public bool EhSuspeita()
    {
        // Sessão muito longa sem atividade
        if (Ativa && TempoDesdeUltimaAtividade.TotalHours > 24)
            return true;

        // Sessão extremamente longa
        if (Duracao.TotalDays > 7)
            return true;

        // User Agent suspeito (muito simples ou contém palavras-chave de bots)
        var palavrasChaveSuspeitas = new[] { "bot", "crawler", "spider", "scraper", "curl", "wget" };
        if (palavrasChaveSuspeitas.Any(palavra => AgenteUsuario.ToLowerInvariant().Contains(palavra)))
            return true;

        return false;
    }

    /// <summary>
    /// Obtém informações resumidas da sessão para logs
    /// </summary>
    public string ObterResumo()
    {
        var status = Ativa ? "Ativa" : "Inativa";
        var duracao = Duracao.TotalMinutes < 60 
            ? $"{Duracao.TotalMinutes:F0}min" 
            : $"{Duracao.TotalHours:F1}h";

        return $"{status} | {duracao} | {EnderecoIp} | {InfoDispositivo}";
    }

    /// <summary>
    /// Extrai informações básicas do dispositivo a partir do User Agent
    /// </summary>
    private static string ExtrairInfoDispositivo(string agenteUsuario)
    {
        if (string.IsNullOrWhiteSpace(agenteUsuario))
            return "Desconhecido";

        var ua = agenteUsuario.ToLowerInvariant();

        // Detectar sistema operacional
        string os = "Desconhecido";
        if (ua.Contains("windows")) os = "Windows";
        else if (ua.Contains("mac os")) os = "macOS";
        else if (ua.Contains("linux")) os = "Linux";
        else if (ua.Contains("android")) os = "Android";
        else if (ua.Contains("ios") || ua.Contains("iphone") || ua.Contains("ipad")) os = "iOS";

        // Detectar navegador
        string navegador = "Desconhecido";
        if (ua.Contains("chrome") && !ua.Contains("edg")) navegador = "Chrome";
        else if (ua.Contains("firefox")) navegador = "Firefox";
        else if (ua.Contains("safari") && !ua.Contains("chrome")) navegador = "Safari";
        else if (ua.Contains("edg")) navegador = "Edge";
        else if (ua.Contains("opera")) navegador = "Opera";

        // Detectar tipo de dispositivo
        string tipoDispositivo = "Desktop";
        if (ua.Contains("mobile") || ua.Contains("android") || ua.Contains("iphone"))
            tipoDispositivo = "Mobile";
        else if (ua.Contains("tablet") || ua.Contains("ipad"))
            tipoDispositivo = "Tablet";

        return $"{navegador} em {os} ({tipoDispositivo})";
    }

    /// <summary>
    /// Verifica se a sessão pertence ao mesmo dispositivo/localização
    /// </summary>
    public bool EhMesmoDispositivo(string enderecoIp, string agenteUsuario)
    {
        if (string.IsNullOrWhiteSpace(enderecoIp) || string.IsNullOrWhiteSpace(agenteUsuario))
            return false;

        // Comparação exata de IP e User Agent
        return EnderecoIp.Equals(enderecoIp.Trim(), StringComparison.OrdinalIgnoreCase) &&
               AgenteUsuario.Equals(agenteUsuario.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Verifica se a sessão é de um dispositivo móvel
    /// </summary>
    public bool EhDispositivoMovel()
    {
        return InfoDispositivo?.Contains("Mobile") == true || InfoDispositivo?.Contains("Tablet") == true;
    }

    /// <summary>
    /// Força o encerramento da sessão (para casos de segurança)
    /// </summary>
    public void ForcarEncerramento(string motivo = "Encerramento forçado")
    {
        Invalidar();
        // Em uma implementação completa, poderia registrar o motivo em um log de auditoria
    }
}