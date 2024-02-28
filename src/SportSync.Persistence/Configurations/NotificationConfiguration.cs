using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;

namespace SportSync.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(notification => notification.Id);
        builder.Property(notification => notification.Id).ValueGeneratedNever();

        builder.OwnsOne(notification => notification.Actions, actionsBuilder =>
        {
            actionsBuilder.WithOwner();

            actionsBuilder.Property(actions => actions.Value)
                .HasColumnName(nameof(Notification.Actions))
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(notification => notification.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(notification => notification.CreatedOnUtc).IsRequired();

        builder.Property(notification => notification.ModifiedOnUtc);

        builder.Property(notification => notification.DeletedOnUtc);

        builder.Property(notification => notification.Deleted).HasDefaultValue(false);

        builder.HasQueryFilter(notification => !notification.Deleted);

        builder.ToTable("Notifications");
    }
}