using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Application.DTOs;
using SistemaGestaoEscolar.Auth.Application.Interfaces;
using System.Security.Claims;

namespace SistemaGestaoEscolar.Auth.Infrastructure.Controllers;

/// <summary>
/// Controller para operações de autenticação e autorização.
/// Implementa endpoints RESTful seguindo princípios de Clean Architecture.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthApplicationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthApplicationService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Realiza login do usuário
    /// </summary>
    /// <param name="loginDto">Dados de login</param>
    /// <returns>Token de acesso e informações do usuário</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            // Enriquecer DTO com informações da requisição
            loginDto.EnderecoIp = GetClientIpAddress();
            loginDto.AgenteUsuario = Request.Headers.UserAgent.ToString();

            var result = await _authService.LoginAsync(loginDto);
            
            _logger.LogInformation("Login realizado com sucesso para usuário: {UserId}", result.Usuario.Id);
            
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex,"Tentativa de login não autorizada");
            return Unauthorized(new ProblemDetails
            {
                Title = "Login Failed",
                Detail = ex.Message,
                Status = StatusCodes.Status401Unauthorized
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError( ex, "Dados de login inválidos");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Login Data",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante login");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Realiza logout do usuário
    /// </summary>
    /// <returns>Confirmação de logout</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _authService.LogoutAsync(userId);
            
            _logger.LogInformation("Logout realizado com sucesso para usuário: {UserId}", userId);
            
            return Ok(new { message = "Logout realizado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante logout");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Renova o token de acesso usando refresh token
    /// </summary>
    /// <param name="refreshTokenDto">Dados do refresh token</param>
    /// <returns>Novos tokens de acesso</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        try
        {
            // Enriquecer DTO com informações da requisição
            refreshTokenDto.EnderecoIp = GetClientIpAddress();
            refreshTokenDto.AgenteUsuario = Request.Headers.UserAgent.ToString();

            var result = await _authService.RefreshTokenAsync(refreshTokenDto);
            
            _logger.LogInformation("Token renovado com sucesso para usuário: {UserId}", result.Usuario.Id);
            
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Tentativa de renovação não autorizada");
            return Unauthorized(new ProblemDetails
            {
                Title = "Token Refresh Failed",
                Detail = ex.Message,
                Status = StatusCodes.Status401Unauthorized
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Dados de refresh inválidos");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Refresh Data",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante renovação de token");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Altera a senha do usuário autenticado
    /// </summary>
    /// <param name="changePasswordDto">Dados para alteração de senha</param>
    /// <returns>Confirmação da alteração</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _authService.ChangePasswordAsync(userId, changePasswordDto);
            
            _logger.LogInformation("Senha alterada com sucesso para usuário: {UserId}", userId);
            
            return Ok(new { message = "Senha alterada com sucesso" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Tentativa não autorizada de alteração de senha");
            return Unauthorized(new ProblemDetails
            {
                Title = "Password Change Failed",
                Detail = ex.Message,
                Status = StatusCodes.Status401Unauthorized
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Dados inválidos para alteração de senha:");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Password Data",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante alteração de senha");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Solicita recuperação de senha
    /// </summary>
    /// <param name="forgotPasswordDto">Email para recuperação</param>
    /// <returns>Confirmação do envio</returns>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            await _authService.ForgotPasswordAsync(forgotPasswordDto);
            
            // Sempre retorna sucesso por segurança (não revelar se email existe)
            return Ok(new { message = "Se o email existir, você receberá instruções para recuperação da senha" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Dados inválidos para recuperação de senha");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Email",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante solicitação de recuperação de senha");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Redefine a senha usando token de recuperação
    /// </summary>
    /// <param name="resetPasswordDto">Dados para redefinição de senha</param>
    /// <returns>Confirmação da redefinição</returns>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        try
        {
            await _authService.ResetPasswordAsync(resetPasswordDto);
            
            return Ok(new { message = "Senha redefinida com sucesso" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Token inválido para reset de senha");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Reset Token",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Dados inválidos para reset de senha");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Password Data",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante reset de senha");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Confirma o email do usuário
    /// </summary>
    /// <param name="email">Email a ser confirmado</param>
    /// <param name="token">Token de confirmação</param>
    /// <returns>Confirmação da verificação</returns>
    [HttpGet("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        try
        {
            await _authService.ConfirmEmailAsync(email, token);
            
            return Ok(new { message = "Email confirmado com sucesso" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Token inválido para confirmação de emaiL");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Confirmation Token",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante confirmação de email");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Reenvia email de confirmação
    /// </summary>
    /// <param name="email">Email para reenvio</param>
    /// <returns>Confirmação do reenvio</returns>
    [HttpPost("resend-confirmation")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendEmailConfirmation([FromBody] string email)
    {
        try
        {
            await _authService.ResendEmailConfirmationAsync(email);
            
            // Sempre retorna sucesso por segurança
            return Ok(new { message = "Se o email existir e não estiver confirmado, você receberá um novo link de confirmação" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante reenvio de confirmação");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Obtém informações do usuário atual
    /// </summary>
    /// <returns>Informações do usuário</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userId = GetCurrentUserId();
            var userInfo = await _authService.GetCurrentUserAsync(userId);
            
            return Ok(userInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter informações do usuário atual");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Verifica se um email está disponível
    /// </summary>
    /// <param name="email">Email a ser verificado</param>
    /// <returns>Disponibilidade do email</returns>
    [HttpGet("check-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckEmailAvailability([FromQuery] string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Email",
                    Detail = "Email é obrigatório",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var isAvailable = await _authService.IsEmailAvailableAsync(email);
            
            return Ok(new { available = isAvailable });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar disponibilidade do email");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Valida a força de uma senha
    /// </summary>
    /// <param name="password">Senha a ser validada</param>
    /// <returns>Informações sobre a força da senha</returns>
    [HttpPost("validate-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PasswordStrengthDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidatePasswordStrength([FromBody] string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Password",
                    Detail = "Senha é obrigatória",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var strength = await _authService.ValidatePasswordStrengthAsync(password);
            
            return Ok(strength);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar força da senha");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Invalida todas as sessões do usuário atual
    /// </summary>
    /// <returns>Confirmação da invalidação</returns>
    [HttpPost("invalidate-sessions")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> InvalidateAllSessions()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _authService.InvalidateAllSessionsAsync(userId);
            
            _logger.LogInformation("Todas as sessões invalidadas para usuário: {UserId}", userId);
            
            return Ok(new { message = "Todas as sessões foram invalidadas" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao invalidar sessões");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    #region Helper Methods

    /// <summary>
    /// Obtém o ID do usuário atual a partir do token JWT
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Token inválido ou usuário não identificado");
        }

        return userId;
    }

    /// <summary>
    /// Obtém o endereço IP do cliente
    /// </summary>
    private string GetClientIpAddress()
    {
        // Verificar headers de proxy primeiro
        var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(realIp))
        {
            return realIp;
        }

        // Fallback para IP da conexão
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    #endregion
}