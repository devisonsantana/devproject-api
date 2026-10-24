using DevProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevProject.Infrastructure.Persistence.Configurations;

public sealed class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.ToTable("Tasks");
        builder.HasKey(task => task.Id);

        builder.Property(task => task.Title)
            .HasMaxLength(300)
            .IsRequired();
        builder.Property(task => task.Description).HasMaxLength(4_000);
        builder.Property(task => task.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        builder.Property(task => task.Priority)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        builder.Property(task => task.CreatedAt).IsRequired();

        builder.HasIndex(task => new { task.FeatureId, task.Order, task.Id });
        builder.HasIndex(task => new { task.FeatureId, task.Status });
        builder.HasIndex(task => task.DueDate);
    }
}
