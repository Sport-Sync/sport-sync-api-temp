using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Core.Primitives.Result;
using SportSync.Domain.Enumerations;
using SportSync.Domain.Repositories;

namespace SportSync.Application.Matches.SendAnnouncementToFriends;

public class SendAnnouncementToFriendsRequestHandler : IRequestHandler<SendAnnouncementToFriendsInput, Result>
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IUserRepository _userRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SendAnnouncementToFriendsRequestHandler(
        IUserIdentifierProvider userIdentifierProvider, 
        IUserRepository userRepository, 
        IMatchRepository matchRepository, 
        IEventRepository eventRepository, 
        IUnitOfWork unitOfWork)
    {
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
        _matchRepository = matchRepository;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SendAnnouncementToFriendsInput request, CancellationToken cancellationToken)
    {
        var maybeUser = await _userRepository.GetByIdAsync(_userIdentifierProvider.UserId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }

        var maybeMatch = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);

        if (maybeMatch.HasNoValue)
        {
            throw new DomainException(DomainErrors.Match.NotFound);
        }

        var match = maybeMatch.Value;
        var user = maybeUser.Value;

        var notifyResult = user.NotifyFriendsAboutMatchAnnouncement(match);

        if (notifyResult.IsFailure)
        {
            return notifyResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}