using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportSync.Domain.Entities;

namespace SportSync.Persistence.Configurations
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(user => user.Id);
            builder.Property(user => user.Id).ValueGeneratedNever();

            builder.HasIndex(x => x.Email).IsUnique().HasFilter("Deleted = 0");

            builder.Property<string>("_passwordHash")
                .HasField("_passwordHash")
                .HasColumnName("PasswordHash")
                .IsRequired();

            builder.Property(user => user.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();

            builder.Property(user => user.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.OwnsOne(user => user.Phone, phoneBuilder =>
            {
                phoneBuilder.WithOwner();

                phoneBuilder.Property(phone => phone.Value)
                    .HasColumnName(nameof(User.Phone))
                    .HasMaxLength(20)
                    .IsRequired();

                phoneBuilder.HasIndex(phone => phone.Value).IsUnique().HasFilter("Deleted = 0");
            });

            builder.Property(user => user.Email)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(user => user.HasProfileImage).HasDefaultValue(false);

            builder.Property(user => user.CreatedOnUtc).IsRequired();

            builder.Property(user => user.ModifiedOnUtc);

            builder.Property(user => user.DeletedOnUtc);

            builder.Property(user => user.Deleted).HasDefaultValue(false);

            builder.HasQueryFilter(user => !user.Deleted);

            builder.HasIndex(user => user.FirstName);
            builder.HasIndex(user => user.LastName);

            builder.ToTable("Users");
        }
    }
}
