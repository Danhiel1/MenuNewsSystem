using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Persistence.Configurations;

public class NewsConfiguration : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.ToTable("News");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .ValueGeneratedOnAdd();

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(int.MaxValue);

        builder.Property(n => n.Content)
            .IsRequired()
            .HasMaxLength(int.MaxValue);
    }
}
