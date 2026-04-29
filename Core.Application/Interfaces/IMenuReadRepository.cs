using Core.Application.ReadModels;

namespace Core.Application.Interfaces
{
    public interface IMenuReadRepository
    {
        Task<IEnumerable<MenuReadModel>> GetAllAsync();
        Task<MenuReadModel?> GetByIdAsync(int id);
    }
}
