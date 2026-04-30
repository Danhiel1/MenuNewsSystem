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
    public class MenuRepository : GenericRepository<Menu>, IMenuRepository
    {
        public MenuRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<Menu?> GetMenuWithNewsAsync(int menuId)
        {
           return await _context.Menus
                .Include(m => m.MenuNews)// Include MenuNews để lấy thông tin liên kết giữa Menu và News
                .ThenInclude(mn=> mn.News)// Include News để lấy thông tin chi tiết của News
                .AsSplitQuery() // Chia nhỏ câu lệnh Query
                .FirstOrDefaultAsync(m => m.Id == menuId);

        }
        public async Task<bool> IsNameUniqueAsync(string name)
        { 
            return !await _context.Menus.AnyAsync(m => m.Name == name);
        }
    }
}
