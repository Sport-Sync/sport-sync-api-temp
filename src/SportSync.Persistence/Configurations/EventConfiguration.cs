using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;

namespace SportSync.Persistence.Configurations;

internal class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(ev => ev.Id);
        builder.Property(ev => ev.Id).ValueGeneratedNever();

        builder.Property(ev => ev.Name).IsRequired();
        builder.Property(ev => ev.Address).IsRequired();
        builder.Property(ev => ev.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(ev => ev.NumberOfPlayers).IsRequired();
        builder.Property(ev => ev.Notes);

        builder.Property(ev => ev.CreatedOnUtc).IsRequired();

        builder.Property(ev => ev.ModifiedOnUtc);

        builder.Property(ev => ev.DeletedOnUtc);

        builder.Property(ev => ev.Deleted).HasDefaultValue(false);

        builder.HasQueryFilter(ev => !ev.Deleted);

        builder.ToTable("Events");
    }
}