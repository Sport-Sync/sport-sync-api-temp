using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Events;
using SportSync.Domain.DomainEvents;
using SportSync.Domain.Repositories;
using SportSync.Domain.Services;

namespace SportSync.Application.FriendshipRequests.DomainEvents;

internal sealed class CreateFriendshipOnFriendshipRequestAcceptedDomainEventHandler : IDomainEventHandler<FriendshipRequestAcceptedDomainEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFriendshipOnFriendshipRequestAcceptedDomainEventHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(FriendshipRequestAcceptedDomainEvent notification, CancellationToken cancellationToken)
    {
        var friendshipService = new FriendshipService(_userRepository);

        await friendshipService.CreateFriendshipAsync(notification.FriendshipRequest);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}