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
            .WithMany(x => x.Termins)
            .HasForeignKey(termin => termin.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(termin => termin.EventName).IsRequired();
        builder.Property(termin => termin.Address).IsRequired();
        builder.Property(termin => termin.Price).IsRequired();
        builder.Property(termin => termin.NumberOfPlayers).IsRequired();
        builder.Property(termin => termin.Notes);

        builder.Property(termin => termin.Date)
            .HasConversion<DateOnlyConverter>()
            .IsRequired();

        builder.Property(termin => termin.StartTimeUtc)
            .HasConversion<TimeOnlyConverter>()
            .IsRequired();

        builder.Property(termin => termin.EndTimeUtc)
            .HasConversion<TimeOnlyConverter>()
            .IsRequired();

        builder.Property(termin => termin.CreatedOnUtc).IsRequired();

        builder.Property(termin => termin.ModifiedOnUtc);

        builder.Property(termin => termin.DeletedOnUtc);

        builder.Property(termin => termin.Deleted).HasDefaultValue(false);

        builder.HasQueryFilter(termin => !termin.Deleted);
    }
}