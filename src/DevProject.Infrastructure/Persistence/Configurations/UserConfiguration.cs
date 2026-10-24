using DevProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevProject.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Name)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(user => user.Email)
            .HasMaxLength(320)
            .IsRequired();
        builder.Property(user => user.NormalizedEmail)
            .HasMaxLength(320)
            .IsRequired();
        builder.Property(user => user.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();
        builder.Property(user => user.CreatedAt).IsRequired();

        builder.HasIndex(user => user.NormalizedEmail).IsUnique();
    }
}
