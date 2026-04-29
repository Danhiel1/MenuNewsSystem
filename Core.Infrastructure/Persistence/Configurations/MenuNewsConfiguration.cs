using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Persistence.Configurations;

/// <summary>
/// TẠI SAO bảng join MenuNews cần config riêng?
/// 
/// 1. COMPOSITE KEY: Bảng này có 2 cột làm Primary Key (MenuId + NewsId)
///    EF Core KHÔNG THỂ tự detect composite key → phải config thủ công
/// 
/// 2. RELATIONSHIPS: Cần chỉ rõ:
///    - MenuNews.MenuId → FK đến Menus.Id
///    - MenuNews.NewsId → FK đến News.Id
///    - 1 Menu có nhiều MenuNews (.WithMany)
///    - 1 MenuNews chỉ thuộc 1 Menu (.HasOne)
/// </summary>
public class MenuNewsConfiguration : IEntityTypeConfiguration<MenuNews>
{
    public void Configure(EntityTypeBuilder<MenuNews> builder)
    {
        builder.ToTable("MenuNews");

        // Composite primary key — 2 cột làm PK
        builder.HasKey(mn => new { mn.MenuId, mn.NewsId });

        // Relationship: MenuNews → Menu (nhiều-1)
        builder.HasOne(mn => mn.Menu)
            .WithMany(m => m.MenuNews)
            .HasForeignKey(mn => mn.MenuId);

        // Relationship: MenuNews → News (nhiều-1)
        builder.HasOne(mn => mn.News)
            .WithMany(n => n.MenuNews)
            .HasForeignKey(mn => mn.NewsId);

        // Index khớp với DB — tăng tốc query theo NewsId
        builder.HasIndex(mn => mn.NewsId)
            .HasDatabaseName("IX_MenuNews_NewsId");
    }
}
