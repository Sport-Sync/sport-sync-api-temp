using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;

namespace SportSync.Persistence.Configurations;

internal class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(match => match.Id);
        builder.Property(match => match.Id).ValueGeneratedNever();

        builder.HasOne<Event>()
            .WithMany()
            .HasForeignKey(match => match.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Schedule)
            .WithMany()
            .HasForeignKey(match => match.ScheduleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(match => match.EventName).IsRequired();
        builder.Property(match => match.Address).IsRequired();
        builder.Property(match => match.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(match => match.NumberOfPlayersExpected).IsRequired();
        builder.Property(match => match.Notes);

        builder.Property(match => match.Date).IsRequired();

        builder.Property(match => match.StartTime).IsRequired();

        builder.Property(match => match.EndTime).IsRequired();

        builder.Property(match => match.CreatedOnUtc).IsRequired();

        builder.Property(match => match.ModifiedOnUtc);

        builder.Property(match => match.DeletedOnUtc);

        builder.Property(match => match.Deleted).HasDefaultValue(false);

        builder.HasQueryFilter(match => !match.Deleted);

        builder.HasIndex(user => user.Date);
        builder.HasIndex(user => user.Status);

        builder.ToTable("Matches");
    }
}