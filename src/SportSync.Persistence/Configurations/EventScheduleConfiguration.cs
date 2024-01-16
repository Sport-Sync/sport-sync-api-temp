using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;
using SportSync.Persistence.Converters;

namespace SportSync.Persistence.Configurations;

internal class EventScheduleConfiguration : IEntityTypeConfiguration<EventSchedule>
{
    public void Configure(EntityTypeBuilder<EventSchedule> builder)
    {
        builder.HasKey(sch => sch.Id);

        builder.HasOne<Event>()
            .WithMany(x => x.Schedules)
            .HasForeignKey(sch => sch.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Property(termin => termin.RepeatWeekly).IsRequired();
        builder.Property(termin => termin.DayOfWeek).IsRequired();

        builder.Property(termin => termin.StartDate)
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