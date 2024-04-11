using System.Linq.Expressions;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Types;
using SportSync.Domain.ValueObjects;

namespace SportSync.Domain.Repositories;

public interface IUserRepository : IQueryableRepository<User, UserType>
{
    Task<Maybe<User>> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<User>> GetByIdsAsync(List<Guid> userIds, CancellationToken cancellationToken);
    Task<Maybe<User>> GetByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<bool> IsPhoneUniqueAsync(PhoneNumber phone);
    void Insert(User user);
    IQueryable<User> GetQueryableWhere(Expression<Func<User, bool>> predicate);
}