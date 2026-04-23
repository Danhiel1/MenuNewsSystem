using Core.Application.Interfaces;
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
        public UpdateMenuCommandHandler(IMenuRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = await _repository.GetByIdAsync(request.Id);
            if (menu == null) return false;
            menu.Name = request.Name;
            menu.Description = request.Description;
            return await _repository.UpdateMenuAsync(menu);
        }
    }
}
