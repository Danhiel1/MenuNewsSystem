using Core.Application.Interfaces;
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
        public DeleteMenuCommandHandler(IMenuRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteMenuAsync(request.Id);
        }
    }

}
