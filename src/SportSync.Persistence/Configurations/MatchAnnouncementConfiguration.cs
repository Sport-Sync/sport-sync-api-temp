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
        
        builder.Property(announcement => announcement.AnnouncementType);

        builder.Property(announcement => announcement.CreatedOnUtc).IsRequired();

        builder.Property(announcement => announcement.ModifiedOnUtc);

        builder.ToTable("MatchAnnouncements");
    }
}