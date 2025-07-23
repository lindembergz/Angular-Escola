using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaGestaoEscolar.Auth.Application.DTOs;
using SistemaGestaoEscolar.Auth.Application.Interfaces;
using SistemaGestaoEscolar.Auth.Domain.Entities;
using SistemaGestaoEscolar.Auth.Domain.Repositories;
using SistemaGestaoEscolar.Auth.Domain.Services;
using SistemaGestaoEscolar.Auth.Domain.ValueObjects;
using SistemaGestaoEscolar.Shared.Infrastructure.Authorization;
using System.Security.Claims;

namespace SistemaGestaoEscolar.Auth.Infrastructure.Controllers;

/// <summary>
/// Controller para gerenciamento de usuários.
/// Permite criação, edição e administração de contas de usuário.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class UserManagementController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IAuthDomainService _authDomainService;
    private readonly ILogger<UserManagementController> _logger;

    public UserManagementController(
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        IAuthDomainService authDomainService,
        ILogger<UserManagementController> logger)
    {
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _authDomainService = authDomainService;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo usuário no sistema
    /// </summary>
    /// <param name="createUserDto">Dados do usuário a ser criado</param>
    /// <returns>Informações do usuário criado</returns>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.UserManagement)]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de usuário com email: {Email}", createUserDto.Email);

            // 1. Validar se o email já existe
            var emailExists = await _userRepository.ExistePorEmailAsync(createUserDto.Email);
            if (emailExists)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Email Already Exists",
                    Detail = "Já existe um usuário com este email",
                    Status = StatusCodes.Status409Conflict
                });
            }

            // 2. Validar se o usuário atual pode criar usuários com este papel
            var currentUserId = GetCurrentUserId();
            var currentUser = await _userRepository.ObterPorIdAsync(currentUserId);
            var targetRole = UserRole.FromCode(createUserDto.Role);

            if (currentUser == null || !_authDomainService.PodeGerenciarUsuario(currentUser, null))
            {
                return Forbid();
            }

            // 3. Validar senha
            var passwordValidation = _passwordHashService.ValidatePassword(createUserDto.Password);
            if (!passwordValidation.IsValid)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Password",
                    Detail = string.Join(", ", passwordValidation.Errors),
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // 4. Criar objetos de valor
            var email = new Email(createUserDto.Email);
            var password = new Password(createUserDto.Password);

            // 5. Criar usuário
            var user = new User(
                createUserDto.FirstName,
                createUserDto.LastName,
                email,
                password,
                targetRole,
                createUserDto.SchoolId
            );

            // 6. Salvar no banco
            await _userRepository.AdicionarAsync(user);
            await _userRepository.SalvarAlteracoesAsync();

            _logger.LogInformation("Usuário criado com sucesso: {UserId}", user.Id);

            // 7. Retornar informações do usuário criado
            var userInfo = new UserInfoDto
            {
                Id = user.Id,
                NomeCompleto = user.NomeCompleto,
                Email = user.Email.Value,
                CodigoPerfil = user.Perfil.Code,
                NomePerfil = user.Perfil.Name,
                Iniciais = user.Iniciais,
                EmailConfirmado = user.EmailConfirmado,
                UltimoLoginEm = user.UltimoLoginEm,
                EscolaId = user.EscolaId
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userInfo);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos para criação de usuário: {Message}", ex.Message);
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid User Data",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno durante criação de usuário");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Obtém informações de um usuário específico
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Informações do usuário</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.SchoolStaff)]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUser(Guid id)
    {
        try
        {
            var user = await _userRepository.ObterPorIdAsync(id);
            if (user == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User Not Found",
                    Detail = "Usuário não encontrado",
                    Status = StatusCodes.Status404NotFound
                });
            }

            // Verificar se o usuário atual pode visualizar este usuário
            var currentUserId = GetCurrentUserId();
            if (currentUserId != id && !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                // Verificar se é da mesma escola
                var currentUser = await _userRepository.ObterPorIdAsync(currentUserId);
                if (currentUser?.EscolaId != user.EscolaId)
                {
                    return Forbid();
                }
            }

            var userInfo = new UserInfoDto
            {
                Id = user.Id,
                NomeCompleto = user.NomeCompleto,
                Email = user.Email.Value,
                CodigoPerfil = user.Perfil.Code,
                NomePerfil = user.Perfil.Name,
                Iniciais = user.Iniciais,
                EmailConfirmado = user.EmailConfirmado,
                UltimoLoginEm = user.UltimoLoginEm,
                EscolaId = user.EscolaId
            };

            return Ok(userInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter usuário: {UserId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Lista usuários com paginação
    /// </summary>
    /// <param name="page">Página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 20)</param>
    /// <param name="search">Termo de busca</param>
    /// <param name="role">Filtro por papel</param>
    /// <param name="schoolId">Filtro por escola</param>
    /// <returns>Lista paginada de usuários</returns>
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.SchoolStaff)]
    [ProducesResponseType(typeof(PagedResult<UserInfoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? role = null,
        [FromQuery] Guid? schoolId = null)
    {
        try
        {
            // Limitar tamanho da página
            pageSize = Math.Min(pageSize, 100);
            page = Math.Max(page, 1);

            IEnumerable<User> users;
            int totalCount;

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(search))
            {
                users = await _userRepository.BuscarAsync(search, (page - 1) * pageSize, pageSize);
                totalCount = users.Count(); // Aproximação para busca
            }
            else if (schoolId.HasValue)
            {
                (users, totalCount) = await _userRepository.ObterPaginadoPorEscolaAsync(schoolId.Value, page, pageSize);
            }
            else
            {
                (users, totalCount) = await _userRepository.ObterPaginadoAsync(page, pageSize);
            }

            // Filtrar por papel se especificado
            if (!string.IsNullOrWhiteSpace(role))
            {
                users = users.Where(u => u.Perfil.Code.Equals(role, StringComparison.OrdinalIgnoreCase));
            }

            // Verificar permissões de acesso
            var currentUserId = GetCurrentUserId();
            var currentUser = await _userRepository.ObterPorIdAsync(currentUserId);
            
            if (currentUser != null && !User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                // Filtrar apenas usuários da mesma escola
                users = users.Where(u => u.EscolaId == currentUser.EscolaId);
            }

            var userInfos = users.Select(u => new UserInfoDto
            {
                Id = u.Id,
                NomeCompleto = u.NomeCompleto,
                Email = u.Email.Value,
                CodigoPerfil = u.Perfil.Code,
                NomePerfil = u.Perfil.Name,
                Iniciais = u.Iniciais,
                EmailConfirmado = u.EmailConfirmado,
                UltimoLoginEm = u.UltimoLoginEm,
                EscolaId = u.EscolaId
            }).ToList();

            var result = new PagedResult<UserInfoDto>
            {
                Items = userInfos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar usuários");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Atualiza informações básicas de um usuário
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="updateUserDto">Dados para atualização</param>
    /// <returns>Informações atualizadas do usuário</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.UserManagement)]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await _userRepository.ObterPorIdAsync(id);
            if (user == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User Not Found",
                    Detail = "Usuário não encontrado",
                    Status = StatusCodes.Status404NotFound
                });
            }

            // Atualizar informações básicas
            user.AtualizarInformacoesBasicas(updateUserDto.FirstName, updateUserDto.LastName);

            // Atualizar papel se especificado e permitido
            if (!string.IsNullOrWhiteSpace(updateUserDto.Role))
            {
                var newRole = UserRole.FromCode(updateUserDto.Role);
                var currentUserId = GetCurrentUserId();
                var currentUser = await _userRepository.ObterPorIdAsync(currentUserId);

                if (currentUser != null && _authDomainService.CanChangeRole(currentUser, user, newRole))
                {
                    user.AtualizarPerfil(newRole);
                }
            }

            _userRepository.Atualizar(user);
            await _userRepository.SalvarAlteracoesAsync();

            _logger.LogInformation("Usuário atualizado com sucesso: {UserId}", user.Id);

            var userInfo = new UserInfoDto
            {
                Id = user.Id,
                NomeCompleto = user.NomeCompleto,
                Email = user.Email.Value,
                CodigoPerfil = user.Perfil.Code,
                NomePerfil = user.Perfil.Name,
                Iniciais = user.Iniciais,
                EmailConfirmado = user.EmailConfirmado,
                UltimoLoginEm = user.UltimoLoginEm,
                EscolaId = user.EscolaId
            };

            return Ok(userInfo);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos para atualização de usuário: {Message}", ex.Message);
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid User Data",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário: {UserId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Ativa ou desativa um usuário
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="activate">True para ativar, false para desativar</param>
    /// <returns>Confirmação da operação</returns>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = AuthorizationPolicies.UserManagement)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleUserStatus(Guid id, [FromBody] bool activate)
    {
        try
        {
            var user = await _userRepository.ObterPorIdAsync(id);
            if (user == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User Not Found",
                    Detail = "Usuário não encontrado",
                    Status = StatusCodes.Status404NotFound
                });
            }

            if (activate)
            {
                user.Ativar();
            }
            else
            {
                user.Desativar();
            }

            _userRepository.Atualizar(user);
            await _userRepository.SalvarAlteracoesAsync();

            var action = activate ? "ativado" : "desativado";
            _logger.LogInformation("Usuário {Action}: {UserId}", action, user.Id);

            return Ok(new { message = $"Usuário {action} com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status do usuário: {UserId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Erro interno do servidor",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Desbloqueia um usuário
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Confirmação da operação</returns>
    [HttpPost("{id:guid}/unlock")]
    [Authorize(Policy = AuthorizationPolicies.UserManagement)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnlockUser(Guid id)
    {
        try
        {
            var user = await _userRepository.ObterPorIdAsync(id);
            if (user == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User Not Found",
                    Detail = "Usuário não encontrado",
                    Status = StatusCodes.Status404NotFound
                });
            }

            user.Desbloquear();

            _userRepository.Atualizar(user);
            await _userRepository.SalvarAlteracoesAsync();

            _logger.LogInformation("Usuário desbloqueado: {UserId}", user.Id);

            return Ok(new { message = "Usuário desbloqueado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desbloquear usuário: {UserId}", id);
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

    #endregion
}

/// <summary>
/// DTO para criação de usuário
/// </summary>
public record CreateUserDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public Guid? SchoolId { get; init; }
}

/// <summary>
/// DTO para atualização de usuário
/// </summary>
public record UpdateUserDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Role { get; init; }
}

/// <summary>
/// Resultado paginado
/// </summary>
public record PagedResult<T>
{
    public IEnumerable<T> Items { get; init; } = Array.Empty<T>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}