using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Persistence.Configurations;
public class MenuNewsConfiguration : IEntityTypeConfiguration<MenuNews>
{
    public void Configure(EntityTypeBuilder<MenuNews> builder)
    {
        builder.ToTable("MenuNews");
        builder.HasKey(mn => new { mn.MenuId, mn.NewsId });
        builder.HasOne(mn => mn.Menu)
            .WithMany(m => m.MenuNews)
            .HasForeignKey(mn => mn.MenuId);
        builder.HasOne(mn => mn.News)
            .WithMany(n => n.MenuNews)
            .HasForeignKey(mn => mn.NewsId);
        builder.HasIndex(mn => mn.NewsId)
            .HasDatabaseName("IX_MenuNews_NewsId");
    }
}
