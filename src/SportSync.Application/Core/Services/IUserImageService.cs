using SportSync.Domain.Types;

namespace SportSync.Application.Core.Services
{
    public interface IUserImageService
    {
        Task PopulateImageUrls(params UserType[] users);
        Task PopulateImageUrls(params FriendshipRequestType[] friendshipRequestTypes);
    }
}