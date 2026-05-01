using System.Reflection;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Persistence;
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
        // Auto implement class IEntityTypeConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
