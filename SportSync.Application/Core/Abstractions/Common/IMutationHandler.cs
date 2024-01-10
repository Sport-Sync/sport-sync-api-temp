namespace SportSync.Application.Core.Abstractions.Common;

public interface IMutationHandler<TInput, TOutput> where TInput : IInput<TOutput>
{
    Task<TOutput> Handle(TInput request, CancellationToken cancellationToken);
}