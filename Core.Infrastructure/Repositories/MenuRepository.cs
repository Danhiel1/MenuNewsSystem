using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly ApplicationDbContext _context;
        public MenuRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreateMenuAsync(Menu menu)
        {
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            return menu.Id; // Trả về ID của Menu vừa tạo
        }

        public async Task<bool> DeleteMenuAsync(int id)
        {
            var menu = _context.Menus.Find(id);
            if (menu == null) return false;
            _context.Menus.Remove(menu);
            var rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Menu>> GetAllMenusAsync()
        {
            return await _context.Menus.ToListAsync();
        }

        public async Task<Menu?> GetByIdAsync(int id)
        {
           return await _context.Menus.FindAsync(id);
        }

        public async Task<Menu?> GetMenuWithNewsAsync(int menuId)
        {
           return await _context.Menus
                .Include(m => m.MenuNews)// Include MenuNews để lấy thông tin liên kết giữa Menu và News
                .ThenInclude(mn=> mn.News)// Include News để lấy thông tin chi tiết của News
                .FirstOrDefaultAsync(m => m.Id == menuId);

        }

        public async Task<bool> UpdateMenuAsync(Menu menu)
        {
            _context.Menus.Update(menu);
            var rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }
    }
}
