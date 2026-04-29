using Core.Application.ReadModels;

namespace Core.Application.Interfaces
{
    /// <summary>
    /// TẠI SAO IMenuReadRepository riêng?
    /// - Giống IMenuRepository vs IGenericRepository
    /// - IGenericReadRepository: CRUD chung cho MongoDB
    /// - IMenuReadRepository: thêm methods đặc thù cho Menu reads (GetAll, GetById)
    /// </summary>
    public interface IMenuReadRepository
    {
        Task<IEnumerable<MenuReadModel>> GetAllAsync();
        Task<MenuReadModel?> GetByIdAsync(int id);
    }
}
