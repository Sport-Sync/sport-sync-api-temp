namespace SportSync.Application.Core.Abstractions.Common;

public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request);
}

public interface IRequest<out TResponse>
{
}