using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;

namespace SportSync.Persistence.Configurations;

internal class MatchAnnouncementConfiguration : IEntityTypeConfiguration<MatchAnnouncement>
{
    public void Configure(EntityTypeBuilder<MatchAnnouncement> builder)
    {
        builder.HasKey(announcement => announcement.Id);
        builder.Property(announcement => announcement.Id).ValueGeneratedNever();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(announcement => announcement.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Match>()
            .WithMany(x => x.Announcements)
            .HasForeignKey(announcement => announcement.MatchId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(announcement => announcement.NumberOfPlayersLimit).IsRequired();
        builder.Property(announcement => announcement.NumberOfPlayersAccepted)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasIndex(x => x.MatchId).IsUnique();

        builder.Property(announcement => announcement.Description);

        builder.Property(announcement => announcement.AnnouncementType);

        builder.Property(announcement => announcement.CreatedOnUtc).IsRequired();

        builder.Property(announcement => announcement.ModifiedOnUtc);

        builder.Property(match => match.DeletedOnUtc);

        builder.Property(match => match.Deleted).HasDefaultValue(false);

        builder.HasQueryFilter(match => !match.Deleted);

        builder.ToTable("MatchAnnouncements");
    }
}