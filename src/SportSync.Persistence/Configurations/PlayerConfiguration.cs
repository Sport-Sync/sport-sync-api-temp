using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;

namespace SportSync.Persistence.Configurations;

internal class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(player => player.Id);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(player => player.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Termin>()
            .WithMany(x => x.Players)
            .HasForeignKey(player => player.TerminId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(player => player.CreatedOnUtc).IsRequired();

        builder.Property(player => player.ModifiedOnUtc);

        builder.Property(player => player.DeletedOnUtc);

        builder.Property(player => player.Deleted).HasDefaultValue(false);

        builder.HasQueryFilter(player => !player.Deleted);
    }
}