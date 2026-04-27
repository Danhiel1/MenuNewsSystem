using Core.Application.DTOs;
using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.ReadModels;
using MediatR;
using MediatR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Features.Menus.Queries
{
    // Query: Chứa dữ liệu đầu vào (nếu có
    public class GetAllMenusQuery : IRequest<IEnumerable<MenuDto>>
    {
    }
    // Handler:Xử lý logic lấy dữ liệu và chuyển đổi sang DTO
    public class GetAllMenusQueryHandler : IRequestHandler<GetAllMenusQuery, IEnumerable<MenuDto>>
    {
        private readonly IMongoCollection<MenuReadModel> _menuCollection;

        public GetAllMenusQueryHandler(IMongoDatabase mongoDatabase)
        {
            _menuCollection = mongoDatabase.GetCollection<MenuReadModel>("Menus");
        }
        public async Task<IEnumerable<MenuDto>> Handle(GetAllMenusQuery request, CancellationToken cancellationToken)
        {
            // Đọc từ MongoDB
            var menus = await _menuCollection.Find(_ => true).ToListAsync(cancellationToken);

            return menus.Select(m => new MenuDto
            {
                Id = m.Id,
                Name = m.Name
            });
        }
    }
}

