using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;

namespace SportSync.Persistence.Configurations;

internal class MatchApplicationConfiguration : IEntityTypeConfiguration<MatchApplication>
{
    public void Configure(EntityTypeBuilder<MatchApplication> builder)
    {
        builder.HasKey(application => application.Id);
        builder.Property(application => application.Id).ValueGeneratedNever();

        builder.HasOne(application => application.AppliedByUser)
            .WithMany()
            .HasForeignKey(application => application.AppliedByUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(application => application.CompletedByUser)
            .WithMany()
            .HasForeignKey(application => application.CompletedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Match>()
            .WithMany()
            .HasForeignKey(application => application.MatchId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(application => application.Accepted).HasDefaultValue(false);

        builder.Property(application => application.Rejected).HasDefaultValue(false);

        builder.Property(application => application.CompletedOnUtc);

        builder.Property(application => application.CreatedOnUtc).IsRequired();

        builder.Property(application => application.ModifiedOnUtc);

        builder.Property(application => application.DeletedOnUtc);

        builder.Property(application => application.Deleted).HasDefaultValue(false);

        builder.HasQueryFilter(application => !application.Deleted);

        builder.ToTable("MatchApplications");
    }
}