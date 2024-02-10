﻿using Microsoft.EntityFrameworkCore;
using SportSync.Application.Core.Abstractions.Data;
using SportSync.Domain.Core.Primitives.Maybe;
using SportSync.Domain.Entities;
using SportSync.Domain.Repositories;
using SportSync.Domain.Types;

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

    public async Task<bool> CheckIfFriendsAsync(User user, User friend)
    {
        return await DbContext.Set<Friendship>()
            .AnyAsync(f =>
                (f.UserId == user.Id && f.FriendId == friend.Id) ||
                (f.FriendId == user.Id && f.UserId == friend.Id));
    }
}