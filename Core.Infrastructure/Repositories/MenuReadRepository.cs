using Core.Application.Interfaces;
using Core.Application.ReadModels;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Repositories
{
    public class MenuReadRepository : IMenuReadRepository
    {
        private readonly IMongoCollection<MenuReadModel> _menuCollection;
        public MenuReadRepository(IMongoDatabase mongoDatabase) 
        {
           _menuCollection = mongoDatabase.GetCollection<MenuReadModel>("Menus");
        }
        public async Task<IEnumerable<MenuReadModel>> GetAllAsync()
        {
            return await _menuCollection.Find(_ => true).ToListAsync();
        }

        public async Task<MenuReadModel?> GetByIdAsync(int id)
        {
           return await _menuCollection.Find(m => m.Id == id).FirstOrDefaultAsync();
        }
    }
}
