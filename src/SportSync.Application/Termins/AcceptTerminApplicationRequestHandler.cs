using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Application.Termins.AcceptTerminApplication;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Termins;

public class AcceptTerminApplicationRequestHandler : IRequestHandler<AcceptTerminApplicationInput, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly ITerminApplicationRepository _terminApplicationRepository;
    private readonly ITerminRepository _terminRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    public AcceptTerminApplicationRequestHandler(
        IUserIdentifierProvider userIdentifierProvider,
        ITerminApplicationRepository terminApplicationRepository,
        ITerminRepository terminRepository,
        IEventRepository eventRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IDateTime dateTime)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _terminApplicationRepository = terminApplicationRepository;
        _terminRepository = terminRepository;
        _eventRepository = eventRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(AcceptTerminApplicationInput request, CancellationToken cancellationToken)
    {
        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        Maybe<TerminApplication> maybeTerminApplication = await _terminApplicationRepository.GetByIdAsync(request.TerminApplicationId, cancellationToken);

        if (maybeTerminApplication.HasNoValue)
        {
            return Result.Failure(DomainErrors.TerminApplication.NotFound);
        }

        Maybe<Termin> maybeTermin = await _terminRepository.GetByIdAsync(maybeTerminApplication.Value.TerminId, cancellationToken);

        if (maybeTermin.HasNoValue)
        {
            return Result.Failure(DomainErrors.TerminApplication.NotFound);
        }

        var terminApplication = maybeTerminApplication.Value;
        var termin = maybeTermin.Value;
        var user = maybeUser.Value;

        await _eventRepository.EnsureUserIsAdminOnEvent(termin.EventId, user.Id, cancellationToken);

        var acceptResult = terminApplication.Accept(user, _dateTime.UtcNow);

        if (acceptResult.IsFailure)
        {
            return Result.Failure(acceptResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}