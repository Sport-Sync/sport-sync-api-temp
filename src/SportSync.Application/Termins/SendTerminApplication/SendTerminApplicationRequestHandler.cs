using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Termins.SendTerminApplication;

public class SendTerminApplicationRequestHandler : IRequestHandler<SendTerminApplicationInput, Result>
{
    private readonly ITerminRepository _terminRepository;

    public SendTerminApplicationRequestHandler(ITerminRepository terminRepository)
    {
        _terminRepository = terminRepository;
    }

    public async Task<Result> Handle(SendTerminApplicationInput request, CancellationToken cancellationToken)
    {
        var maybeTermin = await _terminRepository.GetByIdAsync(request.TerminId, cancellationToken);

        if (maybeTermin.HasNoValue)
        {
            return Result.Failure(DomainErrors.Termin.NotFound);
        }

        return Result.Success();
    }
}