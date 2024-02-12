using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;

namespace SportSync.Persistence.Configurations;

internal class TerminAnnouncementConfiguration : IEntityTypeConfiguration<TerminAnnouncement>
{
    public void Configure(EntityTypeBuilder<TerminAnnouncement> builder)
    {
        builder.HasKey(announcement => announcement.Id);
        builder.Property(announcement => announcement.Id).ValueGeneratedNever();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(announcement => announcement.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Termin>()
            .WithMany(x => x.Announcements)
            .HasForeignKey(announcement => announcement.TerminId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Property(announcement => announcement.AnnouncementType);

        builder.Property(announcement => announcement.CreatedOnUtc).IsRequired();

        builder.Property(announcement => announcement.ModifiedOnUtc);

        builder.ToTable("TerminAnnouncements");
    }
}