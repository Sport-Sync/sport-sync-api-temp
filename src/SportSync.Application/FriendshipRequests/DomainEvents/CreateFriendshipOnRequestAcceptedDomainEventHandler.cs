using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Repositories;
using SportSync.Domain.Services;

namespace SportSync.Application.FriendshipRequests.DomainEvents;

internal sealed class CreateFriendshipOnRequestAcceptedDomainEventHandler : IDomainEventHandler<FriendshipRequestAcceptedDomainEvent>
{
    private readonly IUserRepository _userRepository;

    public CreateFriendshipOnRequestAcceptedDomainEventHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(FriendshipRequestAcceptedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var friendshipService = new FriendshipService(_userRepository);

        await friendshipService.CreateFriendshipAsync(domainEvent.FriendshipRequest, cancellationToken);
    }
}