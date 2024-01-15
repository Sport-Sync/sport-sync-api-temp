using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;
using SportSync.Persistence.Converters;

namespace SportSync.Persistence.Configurations;

internal class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(ev => ev.Id);

        builder.Property(ev => ev.Name).IsRequired();
        builder.Property(ev => ev.Address).IsRequired();
        builder.Property(ev => ev.Price).IsRequired();
        builder.Property(ev => ev.NumberOfPlayers).IsRequired();
        builder.Property(ev => ev.Notes);

        builder.Property(ev => ev.StartingDate)
            .HasConversion<DateOnlyConverter>()
            .IsRequired();

        builder.Property(ev => ev.StartTime)
            .HasConversion<TimeOnlyConverter>()
            .IsRequired();

        builder.Property(ev => ev.EndTime)
            .HasConversion<TimeOnlyConverter>()
            .IsRequired();

        builder.Property(ev => ev.CreatedOnUtc).IsRequired();

        builder.Property(ev => ev.ModifiedOnUtc);

        builder.Property(ev => ev.DeletedOnUtc);

        builder.Property(ev => ev.Deleted).HasDefaultValue(false);

        builder.HasQueryFilter(ev => !ev.Deleted);
    }
}