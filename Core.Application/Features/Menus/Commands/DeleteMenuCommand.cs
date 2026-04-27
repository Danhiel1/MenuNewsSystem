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
    public class DeleteMenuCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, bool>
    {
        private readonly IMenuRepository _repository;
        private readonly IPublishEndpoint _publishEndpoint;
        public DeleteMenuCommandHandler(IMenuRepository repository, IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _publishEndpoint = publishEndpoint;
        }
        public async Task<bool> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            var success = await _repository.DeleteMenuAsync(request.Id);
            if (success)
            {
                await _publishEndpoint.Publish(new MenuDeletedEvent { Id = request.Id }, cancellationToken);
            }
            return success;
        }

    }
}
