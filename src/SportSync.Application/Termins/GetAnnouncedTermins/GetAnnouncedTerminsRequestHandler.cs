using SportSync.Application.Core.Abstractions.Authentication;
using SportSync.Domain.Core.Errors;
using SportSync.Domain.Core.Exceptions;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Termins.GetAnnouncedTermins;

public class GetAnnouncedTerminsRequestHandler : IRequestHandler<GetAnnouncedTerminsInput, GetAnnouncedTerminResponse>
{
    private readonly ITerminRepository _terminRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public GetAnnouncedTerminsRequestHandler(
        ITerminRepository terminRepository,
        IUserIdentifierProvider userIdentifierProvider,
        IUserRepository userRepository)
    {
        _terminRepository = terminRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _userRepository = userRepository;
    }

    public async Task<GetAnnouncedTerminResponse> Handle(GetAnnouncedTerminsInput request, CancellationToken cancellationToken)
    {
        var userId = _userIdentifierProvider.UserId;

        var maybeUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (maybeUser.HasNoValue)
        {
            throw new DomainException(DomainErrors.User.Forbidden);
        }

        var announcedTermins = await _terminRepository.GetAnnouncedTermins(request.Date, cancellationToken);

        var user = maybeUser.Value;
        var response = new GetAnnouncedTerminResponse();

        if (!announcedTermins.Any())
        {
            return response;
        }

        var publicAnnouncements = announcedTermins.Where(x => x.PubliclyAnnounced);
        var privateAnnouncements = announcedTermins.Where(x => !x.PubliclyAnnounced);

        response.Termins.AddRange(publicAnnouncements.Select(TerminType.FromTermin));

        foreach (var termin in privateAnnouncements)
        {
            var announcingUserIds = termin.Announcements.Select(x => x.UserId);
            if (user.Friends.Any(friendId => announcingUserIds.Contains(friendId)))
            {
                response.Termins.Add(TerminType.FromTermin(termin));
            }
        }

        return response;
    }
}