using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;

namespace SportSync.Persistence.Configurations;

internal class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.HasKey(friendship => new
        {
            friendship.UserId,
            friendship.FriendId
        });

        builder.HasOne<User>()
            .WithMany(x => x.FriendInviters)
            .HasForeignKey(friendship => friendship.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<User>()
            .WithMany(x => x.FriendInvitees)
            .HasForeignKey(friendship => friendship.FriendId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(friendship => friendship.CreatedOnUtc).IsRequired();

        builder.Property(friendship => friendship.ModifiedOnUtc);

        builder.Ignore(friendship => friendship.Id);

        builder.ToTable("Friendships");
    }
}