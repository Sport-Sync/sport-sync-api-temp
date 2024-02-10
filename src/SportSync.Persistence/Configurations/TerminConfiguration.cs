using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;
using SportSync.Persistence.Converters;

namespace SportSync.Persistence.Configurations;

internal class TerminConfiguration : IEntityTypeConfiguration<Termin>
{
    public void Configure(EntityTypeBuilder<Termin> builder)
    {
        builder.HasKey(termin => termin.Id);

        builder.HasOne<Event>()
            .WithMany()
            .HasForeignKey(termin => termin.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Schedule)
            .WithMany()
            .HasForeignKey(termin => termin.ScheduleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(termin => termin.EventName).IsRequired();
        builder.Property(termin => termin.Address).IsRequired();
        builder.Property(termin => termin.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(termin => termin.NumberOfPlayersExpected).IsRequired();
        builder.Property(termin => termin.Notes);

        builder.Property(termin => termin.Date).IsRequired();

        builder.Property(termin => termin.StartTime)
            .HasConversion<TimeOnlyConverter>()
            .IsRequired();

        builder.Property(termin => termin.EndTime)
            .HasConversion<TimeOnlyConverter>()
            .IsRequired();

        builder.Property(termin => termin.CreatedOnUtc).IsRequired();

        builder.Property(termin => termin.ModifiedOnUtc);

        builder.Property(termin => termin.DeletedOnUtc);

        builder.Property(termin => termin.Deleted).HasDefaultValue(false);

        builder.HasQueryFilter(termin => !termin.Deleted);

        builder.ToTable("Termins");
    }
}