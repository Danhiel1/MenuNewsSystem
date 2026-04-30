using Core.Application.DTOs;
using Core.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Features.Menus.Queries
{
    // Query cần truyền vào ID của Menu
    public class GetMenuWithNewsQuery : IRequest<MenuDetailDto?>
    {
        public int MenuId { get; set; }
    }
    // Handler để xử lý Query
    public class GetMenuWithNewsQueryHandler : IRequestHandler<GetMenuWithNewsQuery, MenuDetailDto?>
    {
        private readonly IMenuRepository _repository;

        public GetMenuWithNewsQueryHandler(IMenuRepository repository)
        {
            _repository = repository;
        }
    public async Task<MenuDetailDto?> Handle(GetMenuWithNewsQuery request, CancellationToken cancellationToken)
        {
            var menu = await _repository.GetMenuWithNewsAsync(request.MenuId);
            if (menu == null) return null;
            var dto = new MenuDetailDto
            {
                Id = menu.Id,
                Name = menu.Name,
                Description = menu.Description,
                NewsList = menu.MenuNews.Select(mn => new NewsDto
                {
                    Id = mn.News.Id,
                    Title = mn.News.Title,
                  
                }).ToList()
            };
            return dto;
        }
    }
}
