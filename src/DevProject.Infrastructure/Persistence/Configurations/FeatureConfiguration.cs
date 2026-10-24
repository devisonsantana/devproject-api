using DevProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevProject.Infrastructure.Persistence.Configurations;

public sealed class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.ToTable("Features");
        builder.HasKey(feature => feature.Id);

        builder.Property(feature => feature.Name)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(feature => feature.Description).HasMaxLength(2_000);
        builder.Property(feature => feature.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        builder.Property(feature => feature.Order).IsRequired();
        builder.Property(feature => feature.CreatedAt).IsRequired();

        builder.HasMany(feature => feature.Tasks)
            .WithOne()
            .HasForeignKey(task => task.FeatureId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(feature => feature.Tasks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(feature => new { feature.ProjectId, feature.Order, feature.Id });
        builder.HasIndex(feature => new { feature.ProjectId, feature.Status });
    }
}
