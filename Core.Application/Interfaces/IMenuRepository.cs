using Core.Domain.Entities;

namespace Core.Application.Interfaces
{
    /// <summary>
    /// TẠI SAO kế thừa IGenericRepository&lt;Menu&gt;?
    /// - Được HẾT 6 methods CRUD chung (GetByIdAsync, AddAsync, Update, Delete, SaveChangesAsync, GetAllAsync)
    /// - Chỉ cần thêm methods ĐẶC THÙ cho Menu (GetMenuWithNewsAsync, IsNameUniqueAsync)
    /// - Handlers gọi _repository.AddAsync() → tìm thấy từ IGenericRepository&lt;Menu&gt;
    /// - Handlers gọi _repository.GetMenuWithNewsAsync() → tìm thấy từ IMenuRepository
    /// 
    /// TẠI SAO không viết hết trong 1 interface?
    /// - Vi phạm DRY: News, Category... cũng cần AddAsync, Delete — phải copy paste
    /// - Với kế thừa: chỉ viết 1 lần trong IGenericRepository, dùng lại cho tất cả
    /// </summary>
    public interface IMenuRepository : IGenericRepository<Menu>
    {
        // Methods ĐẶC THÙ — chỉ Menu mới có
        Task<Menu?> GetMenuWithNewsAsync(int menuId);
        Task<bool> IsNameUniqueAsync(string name);
    }
}
