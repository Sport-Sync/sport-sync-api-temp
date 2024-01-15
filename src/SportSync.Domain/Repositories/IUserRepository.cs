using SportSync.Domain.Core.Primitives;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;

namespace SportSync.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<Maybe<User>> GetByIdAsync(Guid userId);
    Task<Maybe<User>> GetByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<bool> IsPhoneUniqueAsync(string phone);
    void Insert(User user);
}

public interface IRepository<T> where T : Entity
{
    IQueryable<T> Get();
}