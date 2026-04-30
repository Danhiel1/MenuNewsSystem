namespace Core.Application.Interfaces
{
    /// <summary>
    /// TẠI SAO cần IGenericRepository?
    /// - Đây là interface CHUNG cho mọi entity (Menu, News, Category...)
    /// - Chứa 5 methods CRUD cơ bản mà entity nào cũng cần
    /// - Khi thêm table mới, KHÔNG cần viết lại CRUD — kế thừa interface này là đủ
    /// 
    /// TẠI SAO dùng Generic &lt;T&gt;?
    /// - T là entity bất kỳ: IGenericRepository&lt;Menu&gt;, IGenericRepository&lt;News&gt;...
    /// - Viết 1 lần, dùng cho tất cả entities
    /// 
    /// TẠI SAO "where T : class"?
    /// - EF Core yêu cầu entity phải là reference type (class), không phải int, bool...
    /// </summary>
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveChangesAsync();
    }
}
