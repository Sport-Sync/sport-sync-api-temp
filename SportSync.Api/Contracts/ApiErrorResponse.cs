namespace sport_sync.Contracts;

/// <summary>
/// Represents API an error response.
/// </summary>
public class ApiErrorResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiErrorResponse"/> class.
    /// </summary>
    /// <param name="errors">The enumerable collection of errors.</param>
    public ApiErrorResponse(IReadOnlyCollection<SportSync.Domain.Core.Primitives.Error> errors) => Errors = errors;

    /// <summary>
    /// Gets the errors.
    /// </summary>
    public IReadOnlyCollection<SportSync.Domain.Core.Primitives.Error> Errors { get; }
}