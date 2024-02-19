using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Types;
using SportSync.Domain.ValueObjects;

namespace SportSync.Domain.Repositories;

public interface IUserRepository : IQueryableRepository<User, UserType>
{
    Task<Maybe<User>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Maybe<User>> GetByEmailAsync(string email);
    Task<List<User>> GetByPhoneNumbersAsync(List<PhoneNumber> phoneNumbers, CancellationToken cancellationToken);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<bool> IsPhoneUniqueAsync(PhoneNumber phone);
    void Insert(User user);
}