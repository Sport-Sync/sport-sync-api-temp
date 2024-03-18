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
        builder.Property(sch => sch.Id).ValueGeneratedNever();

        builder.HasOne(x => x.Event)
            .WithMany(x => x.Schedules)
            .HasForeignKey(sch => sch.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Property(match => match.RepeatWeekly).IsRequired();
        builder.Property(match => match.DayOfWeek).IsRequired();

        builder.Property(match => match.StartDate).IsRequired();

        builder.Property(match => match.StartTime)
            .HasConversion<TimeOnlyConverter>()
            .IsRequired();

        builder.Property(match => match.EndTime)
            .HasConversion<TimeOnlyConverter>()
            .IsRequired();

        builder.Property(match => match.CreatedOnUtc).IsRequired();
        builder.Property(match => match.ModifiedOnUtc);
        builder.Property(match => match.DeletedOnUtc);
        builder.Property(match => match.Deleted).HasDefaultValue(false);
        
        builder.HasQueryFilter(match => !match.Deleted);

        builder.ToTable("EventSchedules");
    }
}