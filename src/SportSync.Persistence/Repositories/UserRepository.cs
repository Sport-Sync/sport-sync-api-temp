using System.Linq.Expressions;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.DtoTypes;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Persistence.Repositories;

internal sealed class UserRepository : QueryableGenericRepository<User, UserType>, IUserRepository
{
    public UserRepository(IDbContext dbContext)
        : base(dbContext, UserType.PropertySelector)
    {
    }

    public async Task<bool> IsEmailUniqueAsync(string email) => !await AnyAsync(x => x.Email == email);
    public async Task<bool> IsPhoneUniqueAsync(string phone) => !await AnyAsync(x => x.Phone == phone);
    public async Task<Maybe<User>> GetByEmailAsync(string email) => await FirstOrDefaultAsync(x => x.Email == email);

    public IQueryable<UserType> GetQueryable(Expression<Func<User, bool>> where = null)
    {
        return DbContext.Set<User>()
            .Where(where)
            .Select(UserType.PropertySelector);
    }
}