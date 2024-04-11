using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;
using SportSync.Domain.ValueObjects;

namespace SportSync.Persistence.Repositories;

internal sealed class UserRepository : QueryableGenericRepository<User, UserType>, IUserRepository
{
    public UserRepository(IDbContext dbContext)
        : base(dbContext, UserType.PropertySelector)
    {
    }

    public override async Task<Maybe<User>> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return Maybe<User>.From(await DbContext.Set<User>()
            .Include(x => x.FriendInviters)
            .Include(x => x.FriendInvitees)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken));
    }

    public async Task<List<User>> GetByIdsAsync(List<Guid> userIds, CancellationToken cancellationToken)
    {
        return await DbContext.Set<User>()
            .Where(u => userIds.Contains(u.Id))
        .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email) => !await AnyAsync(x => x.Email == email);

    public async Task<bool> IsPhoneUniqueAsync(PhoneNumber phone) => !await AnyAsync(x => x.Phone.Value == phone);

    public async Task<Maybe<User>> GetByEmailAsync(string email) => await FirstOrDefaultAsync(x => x.Email == email);
}