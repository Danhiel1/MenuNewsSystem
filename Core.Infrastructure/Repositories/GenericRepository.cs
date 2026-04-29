using Core.Application.Interfaces;
using Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repositories
{
    /// <summary>
    /// TẠI SAO cần GenericRepository?
    /// - Implement CRUD 1 lần cho TẤT CẢ entities
    /// - MenuRepository kế thừa class này → không cần viết lại Add, Delete...
    /// - Khi thêm table mới (Category), chỉ cần: GenericRepository&lt;Category&gt; — xong
    /// 
    /// TẠI SAO dùng DbSet&lt;T&gt;?
    /// - _context.Set&lt;T&gt;() trả về DbSet tương ứng với entity T
    /// - T = Menu → _context.Set&lt;Menu&gt;() = _context.Menus
    /// - T = News → _context.Set&lt;News&gt;() = _context.News
    /// 
    /// TẠI SAO "protected"?
    /// - Để class con (MenuRepository) có thể truy cập _context và _dbSet
    /// - Nhưng bên ngoài (Handler) không thể — đảm bảo encapsulation
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// TẠI SAO SaveChangesAsync ở đây?
        /// - Hiện tại để ở repository cho đơn giản
        /// - Ở Điểm 3 (UnitOfWork), sẽ chuyển ra UnitOfWork để quản lý transaction tập trung
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
