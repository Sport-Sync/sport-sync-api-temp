using SportSync.Domain.Types;

namespace SportSync.Application.Core.Services
{
    public interface IUserProfileImageService
    {
        Task PopulateImageUrl(params UserType[] users);
        Task PopulateImageUrl(params FriendshipRequestType[] friendshipRequestTypes);
        Task PopulateImageUrl(params PlayerType[] playerTypes);
    }
}