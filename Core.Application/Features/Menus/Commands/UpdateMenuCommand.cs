using Core.Application.Events;
using Core.Application.Interfaces;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Features.Menus.Commands
{
    public class UpdateMenuCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
    public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, bool>
    {
        private readonly IMenuRepository _repository;
        private readonly IPublishEndpoint _publishEndpoint;
        public UpdateMenuCommandHandler(IMenuRepository repository,IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _publishEndpoint = publishEndpoint;

        }
        public async Task<bool> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = await _repository.GetByIdAsync(request.Id);
            if (menu == null) return false;

            menu.Name = request.Name;
            menu.Description = request.Description;

            var success = await _repository.UpdateMenuAsync(menu);
            if (success)
            {
               
                var updateMessage = new MenuUpdatedEvent
                {
                    Id = request.Id,
                    Name = request.Name,
                    Description = request.Description
                };

                await _publishEndpoint.Publish(updateMessage, cancellationToken);
            }

            return success;
        }
    }
}
