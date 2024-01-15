using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;

namespace SportSync.Persistence.Configurations;

internal class EventMemberConfiguration : IEntityTypeConfiguration<EventMember>
{
    public void Configure(EntityTypeBuilder<EventMember> builder)
    {
        builder.HasKey(member => member.Id);
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(member => member.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Event>()
            .WithMany(x => x.Members)
            .HasForeignKey(member => member.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(member => member.IsAdmin).IsRequired().HasDefaultValue(false);

        builder.Property(member => member.CreatedOnUtc).IsRequired();

        builder.Property(member => member.ModifiedOnUtc);

        builder.Property(member => member.DeletedOnUtc);

        builder.Property(member => member.Deleted).HasDefaultValue(false);

        builder.HasQueryFilter(member => !member.Deleted);
    }
}