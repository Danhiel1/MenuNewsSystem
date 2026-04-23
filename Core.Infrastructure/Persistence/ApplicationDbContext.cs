using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Persistence
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<MenuNews> MenuNews { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure the many-to-many relationship between Menu and News using the MenuNews join entity (Cấu hình khóa chính kép)
            modelBuilder.Entity<MenuNews>()
                .HasKey(mn => new { mn.MenuId, mn.NewsId });
            // Configure the relationships between MenuNews and Menu, and MenuNews and News (Cấu hình quan hệ giữa MenuNews và Menu, và giữa MenuNews và News)
            modelBuilder.Entity<MenuNews>()
                .HasOne(mn => mn.Menu)
                .WithMany(m => m.MenuNews)
                .HasForeignKey(mn => mn.MenuId);

            modelBuilder.Entity<MenuNews>()
                  .HasOne(mn => mn.News)
                  .WithMany(n => n.MenuNews)
                  .HasForeignKey(mn => mn.NewsId);
        }
    }
}
