using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Termins.SendTerminApplication;

public class SendTerminApplicationRequestHandler : IRequestHandler<SendTerminApplicationInput, Result>
{
    private readonly ITerminRepository _terminRepository;
    private readonly ITerminApplicationRepository _terminApplicationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUnitOfWork _unitOfWork;

    public SendTerminApplicationRequestHandler(
        ITerminRepository terminRepository,
        IUserRepository userRepository,
        ITerminApplicationRepository terminApplicationRepository,
        IUserIdentifierProvider userIdentifierProvider, 
        IUnitOfWork unitOfWork)
    {
        _terminRepository = terminRepository;
        _userRepository = userRepository;
        _terminApplicationRepository = terminApplicationRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SendTerminApplicationInput request, CancellationToken cancellationToken)
    {
        Maybe<User> maybeUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            return Result.Failure(DomainErrors.User.Forbidden);
        }

        var maybeTermin = await _terminRepository.GetByIdAsync(request.TerminId, cancellationToken);

        if (maybeTermin.HasNoValue)
        {
            return Result.Failure(DomainErrors.Termin.NotFound);
        }

        var termin = maybeTermin.Value;
        var user = maybeUser.Value;

        var application = termin.ApplyForPlaying(user);

        if (application.IsFailure)
        {
            return Result.Failure(application.Error);
        }

        _terminApplicationRepository.Insert(application.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}