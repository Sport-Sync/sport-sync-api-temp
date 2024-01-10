namespace sport_sync.GraphQL;

public class GraphQLErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        var message = error.Exception?.Message ?? error.Message;

        return error
            .RemoveExtensions()
            .RemoveLocations()
            .RemovePath()
            .WithMessage(message);
    }
}