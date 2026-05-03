using Core.Domain.Entities;

namespace Core.Application.Interfaces
{
    public interface IMenuRepository : IGenericRepository<Menu>
    {
        // Methods Special 
        Task<Menu?> GetMenuWithNewsAsync(int menuId);
        Task<bool> IsNameUniqueAsync(string name);
        Task<bool> IsNameUniqueAsync(string name, int excludeId);

    }
}
