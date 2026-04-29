using System.Reflection;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Persistence;

/// <summary>
/// TẠI SAO dùng ApplyConfigurationsFromAssembly thay vì inline Fluent API?
/// 
/// TRƯỚC (inline — tất cả dồn ở đây):
///   modelBuilder.Entity&lt;MenuNews&gt;().HasKey(...)
///   modelBuilder.Entity&lt;MenuNews&gt;().HasOne(...)
///   modelBuilder.Entity&lt;Menu&gt;().Property(...)
///   // 20 tables = 200+ dòng ở đây → KHÔNG thể maintain
/// 
/// SAU (tách file — mỗi entity 1 file riêng):
///   modelBuilder.ApplyConfigurationsFromAssembly(...)
///   // 1 DÒNG DUY NHẤT — tự động quét tất cả IEntityTypeConfiguration
///   // Thêm table mới? Tạo file XxxConfiguration.cs → tự động được load
///   // Không cần sửa DbContext!
/// 
/// TẠI SAO "partial class"?
/// - Cho phép mở rộng DbContext ở file khác mà không sửa file này
/// - Ví dụ: thêm OnModelCreatingPartial ở file riêng cho config đặc biệt
/// </summary>
public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Menu> Menus { get; set; }
    public virtual DbSet<News> News { get; set; }
    public virtual DbSet<MenuNews> MenuNews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TỰ ĐỘNG quét assembly tìm tất cả class implement IEntityTypeConfiguration
        // → MenuConfiguration, NewsConfiguration, MenuNewsConfiguration được load tự động
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
