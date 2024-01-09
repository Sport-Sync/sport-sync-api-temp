using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface IUserRepository
{
    Task<bool> IsEmailUniqueAsync(string email);
    Task<bool> IsPhoneUniqueAsync(string phone);
    void Insert(User user);
}