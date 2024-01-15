using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;

namespace SportSync.Persistence.Repositories;

internal sealed class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(IDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<bool> IsEmailUniqueAsync(string email) => !await AnyAsync(x => x.Email == email);
    public async Task<bool> IsPhoneUniqueAsync(string phone) => !await AnyAsync(x => x.Phone == phone);
    public async Task<Maybe<User>> GetByEmailAsync(string email) => await FirstOrDefaultAsync(x => x.Email == email);
}