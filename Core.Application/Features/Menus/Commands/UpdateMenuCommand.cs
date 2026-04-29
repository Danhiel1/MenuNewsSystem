using Core.Application.Events;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using MassTransit;
using MediatR;

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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public UpdateMenuCommandHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<bool> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<Menu>();
            var menu = await repo.GetByIdAsync(request.Id);
            if (menu == null) return false;

            // Bắt đầu transaction SAU khi đã check entity tồn tại
            // → không cần transaction cho read-only operations
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                menu.Name = request.Name;
                menu.Description = request.Description;

                repo.Update(menu);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _publishEndpoint.Publish(new MenuUpdatedEvent
                {
                    Id = request.Id,
                    Name = request.Name,
                    Description = request.Description
                }, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
