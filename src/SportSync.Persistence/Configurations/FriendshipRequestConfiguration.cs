using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;

namespace SportSync.Persistence.Configurations;

internal class FriendshipRequestConfiguration : IEntityTypeConfiguration<FriendshipRequest>
{
    public void Configure(EntityTypeBuilder<FriendshipRequest> builder)
    {
        builder.HasKey(friendshipRequest => friendshipRequest.Id);
        builder.Property(friendshipRequest => friendshipRequest.Id).ValueGeneratedNever();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(friendshipRequest => friendshipRequest.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(friendshipRequest => friendshipRequest.FriendId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(friendshipRequest => friendshipRequest.Accepted).HasDefaultValue(false);

        builder.Property(friendshipRequest => friendshipRequest.Rejected).HasDefaultValue(false);

        builder.Property(friendshipRequest => friendshipRequest.CompletedOnUtc);

        builder.Property(friendshipRequest => friendshipRequest.CreatedOnUtc).IsRequired();

        builder.Property(friendshipRequest => friendshipRequest.ModifiedOnUtc);

        builder.Property(friendshipRequest => friendshipRequest.DeletedOnUtc);

        builder.Property(friendshipRequest => friendshipRequest.Deleted).HasDefaultValue(false);

        builder.HasQueryFilter(friendshipRequest => !friendshipRequest.Deleted);

        builder.ToTable("FriendshipRequests");
    }
}