namespace SportSync.Application.Core.Abstractions.Common;

public interface IRequestHandler<TRequest, TOutput> where TRequest : IRequest<TOutput>
{
    Task<TOutput> Handle(TRequest request, CancellationToken cancellationToken);
}

public interface IRequestHandler<TOutput>
{
    Task<TOutput> Handle(CancellationToken cancellationToken);
}