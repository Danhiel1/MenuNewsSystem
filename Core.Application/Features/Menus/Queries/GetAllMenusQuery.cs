using Core.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.DTOs;
using Core.Application.Interfaces;
using MediatR;

namespace Core.Application.Features.Menus.Queries
{
    // Query: Chứa dữ liệu đầu vào (nếu có
    public class GetAllMenusQuery : IRequest<IEnumerable<MenuDto>>
    {
    }
    // Handler:Xử lý logic lấy dữ liệu và chuyển đổi sang DTO
    public class GetAllMenusQueryHandler :IRequestHandler<GetAllMenusQuery, IEnumerable<MenuDto>>
    {
        private readonly IMenuRepository _repository;
        public GetAllMenusQueryHandler(IMenuRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<MenuDto>> Handle(GetAllMenusQuery request, CancellationToken cancellationToken)
        {
            // Chuyển đổi (Map) từ Entity sang DTO để trả về cho người dùng
            var menus = await _repository.GetAllMenusAsync();
            return menus.Select(m => new MenuDto
            {
                Id = m.Id,
                Name = m.Name,
              
            });
        }
    }
}

