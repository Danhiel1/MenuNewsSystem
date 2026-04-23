using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Menus.Commands
{
    // 1. Command: Chứa dữ liệu đầu vào (tương đương DTO)
    public class CreateMenuCommand : IRequest<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    // 2. Handler: Xử lý logic khi nhận được Command
    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, int>
    {
        private readonly IMenuRepository _repository;

        public CreateMenuCommandHandler(IMenuRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = new Menu
            {
                Name = request.Name,
                Description = request.Description
            };

            return await _repository.CreateMenuAsync(menu);
        }
    }
}
