using Core.Application.Events;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using MassTransit;
using MassTransit.Transports;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private readonly IPublishEndpoint _publishEndpoint;
        public CreateMenuCommandHandler(IMenuRepository repository, IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<int> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = new Menu
            {
                Name = request.Name,
                Description = request.Description
            };
            var message = new MenuCreatedEvent
            {
                Id = menu.Id,
                Name = menu.Name,
                Description = menu.Description
            };
            await _publishEndpoint.Publish(message, cancellationToken);
            return menu.Id;
        }
    }
}
