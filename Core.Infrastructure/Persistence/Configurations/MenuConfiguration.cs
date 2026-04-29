using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Persistence.Configurations;

/// <summary>
/// TẠI SAO tách config ra file riêng thay vì để trong DbContext?
/// 
/// 1. SCALABILITY: Nếu 20 tables, OnModelCreating sẽ có 200+ dòng → khó đọc, khó maintain
///    Tách ra file riêng: mỗi entity 1 file ~ 30 dòng → dễ quản lý
/// 
/// 2. SEPARATION OF CONCERNS: Mỗi file chỉ lo mapping cho 1 entity
///    Sửa mapping Menu không ảnh hưởng News
/// 
/// 3. AUTO-DISCOVERY: DbContext gọi ApplyConfigurationsFromAssembly() 
///    → tự động tìm tất cả IEntityTypeConfiguration → không cần sửa DbContext khi thêm table
///
/// TẠI SAO cần HasColumnName?
/// - Khi DB dùng naming convention khác C# (vd: snake_case)
/// - DB: menu_id, menu_name → C#: Id, Name
/// - EF Core cần biết property nào map với column nào
/// - Nếu tên giống nhau (DB: Id → C#: Id) thì KHÔNG CẦN HasColumnName
///   nhưng vẫn nên viết để rõ ràng, nhất là khi làm Database-First
/// </summary>
public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        // Map đến tên bảng trong DB — rõ ràng hơn convention
        builder.ToTable("Menus");

        builder.HasKey(m => m.Id);

        // ValueGeneratedOnAdd = DB tự tạo Id (IDENTITY)
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();
        // Nếu DB dùng snake_case: .HasColumnName("menu_id");

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(int.MaxValue);
        // Nếu DB dùng snake_case: .HasColumnName("menu_name");

        builder.Property(m => m.Description)
            .IsRequired()
            .HasMaxLength(int.MaxValue);
        // Nếu DB dùng snake_case: .HasColumnName("menu_description");
    }
}
