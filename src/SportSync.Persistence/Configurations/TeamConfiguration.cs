using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;


namespace SportSync.Persistence.Configurations;

internal class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(team => team.Id);
        builder.Property(team => team.Id).ValueGeneratedNever();

        builder.HasOne<Event>()
            .WithMany(e => e.Teams)
            .HasForeignKey(team => team.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(team => team.Name).IsRequired();

        builder.Property(team => team.CreatedOnUtc).IsRequired();
        builder.Property(team => team.ModifiedOnUtc);
        builder.Property(team => team.DeletedOnUtc);
        builder.Property(team => team.Deleted).HasDefaultValue(false);

        builder.HasQueryFilter(team => !team.Deleted);

        builder.ToTable("Teams");
    }
}