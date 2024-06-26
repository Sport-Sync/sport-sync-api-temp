﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;

namespace SportSync.Persistence.Configurations;

internal class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(player => player.Id);
        builder.Property(player => player.Id).ValueGeneratedNever();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(player => player.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Match>()
            .WithMany(x => x.Players)
            .HasForeignKey(player => player.MatchId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(player => player.Attending);
        builder.Property(player => player.HasAnnouncedMatch).HasDefaultValue(false);

        builder.Property(player => player.CreatedOnUtc).IsRequired();
        builder.Property(player => player.ModifiedOnUtc);
        builder.Property(player => player.DeletedOnUtc);
        builder.Property(player => player.Deleted).HasDefaultValue(false);

        builder.HasQueryFilter(player => !player.Deleted);

        builder.ToTable("Players");
    }
}