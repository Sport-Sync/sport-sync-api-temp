using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

namespace SportSync.Application.Core.Services
{
    public interface IFriendshipInformationService
    {
        Task<FriendshipInformationType> GetFriendshipInformationForCurrentUser(
            User currentUser,
            User otherUser,
            List<User> currentUserFriends,
            IFriendshipRequestRepository friendshipRequestRepository,
            IUserProfileImageService userProfileImageService,
            CancellationToken cancellationToken);
    }
}