namespace sport_sync.GraphQL;

public class GraphQlErrorFilter : IErrorFilter
{
    private readonly IServiceScopeFactory _scopeFactory;

    public GraphQlErrorFilter(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public IError OnError(IError error)
    {
        var message = error.Exception?.InnerException?.Message ?? error.Exception?.Message ?? error.Message;

        if (error.Exception != null)
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ILogger<GraphQlErrorFilter>>();
            service.LogError(error.Exception, "An exception occurred: {message}", message);
        }

        return error
            .RemoveExtensions()
            .RemoveLocations()
            .RemovePath()
            .WithMessage(message);
    }
}