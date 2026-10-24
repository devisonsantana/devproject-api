using DevProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevProject.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        builder.HasKey(project => project.Id);

        builder.Property(project => project.Name)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(project => project.Description).HasMaxLength(2_000);
        builder.Property(project => project.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        builder.Property(project => project.CreatedAt).IsRequired();

        builder.HasMany(project => project.Features)
            .WithOne()
            .HasForeignKey(feature => feature.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(project => project.Features)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(project => project.Status);
        builder.HasIndex(project => project.CreatedAt);
    }
}
