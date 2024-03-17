using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;

namespace SportSync.Persistence.Configurations;

public class EventInvitationConfiguration : IEntityTypeConfiguration<EventInvitation>
{
    public void Configure(EntityTypeBuilder<EventInvitation> builder)
    {
        builder.HasKey(invitation => invitation.Id);
        builder.Property(invitation => invitation.Id).ValueGeneratedNever();

        builder.HasOne(invitation => invitation.SentByUser)
            .WithMany()
            .HasForeignKey(invitation => invitation.SentByUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(invitation => invitation.SentToUser)
            .WithMany()
            .HasForeignKey(invitation => invitation.SentToUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Event>()
            .WithMany(e => e.Invitations)
            .HasForeignKey(invitation => invitation.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(invitation => invitation.Accepted).HasDefaultValue(false);

        builder.Property(invitation => invitation.Rejected).HasDefaultValue(false);

        builder.Property(invitation => invitation.CompletedOnUtc);

        builder.Property(invitation => invitation.CreatedOnUtc).IsRequired();

        builder.Property(invitation => invitation.ModifiedOnUtc);

        builder.ToTable("EventInvitations");
    }
}