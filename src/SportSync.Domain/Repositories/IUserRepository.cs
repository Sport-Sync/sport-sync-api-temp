using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Types;

namespace SportSync.Domain.Repositories;

public interface IUserRepository : IQueryableRepository<User, UserType>
{
    Task<Maybe<User>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Maybe<User>> GetByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<bool> IsPhoneUniqueAsync(string phone);
    void Insert(User user);
}