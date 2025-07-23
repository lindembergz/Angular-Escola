namespace SistemaGestaoEscolar.Shared.Domain.Exceptions;

/// <summary>
/// Exceção que representa violações de regras de negócio no domínio
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}