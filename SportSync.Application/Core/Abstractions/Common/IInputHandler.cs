namespace SportSync.Application.Core.Abstractions.Common;

public interface IInputHandler<TInput, TOutput> where TInput : IInput<TOutput>
{
    Task<TOutput> Handle(TInput request);
}