namespace SistemaGestaoEscolar.Shared.Application.Interfaces;

public interface IUseCase<in TRequest, TResponse>
{
    Task<TResponse> ExecuteAsync(TRequest request);
}

public interface IUseCase<in TRequest>
{
    Task ExecuteAsync(TRequest request);
}

public interface IQueryUseCase<TResponse>
{
    Task<TResponse> ExecuteAsync();
}