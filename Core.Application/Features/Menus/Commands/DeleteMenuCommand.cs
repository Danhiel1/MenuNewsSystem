using Core.Application.Events;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using MassTransit;
using MediatR;

namespace Core.Application.Features.Menus.Commands
{
    public class DeleteMenuCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public DeleteMenuCommandHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<bool> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<Menu>();
            var menu = await repo.GetByIdAsync(request.Id);
            if (menu == null) return false;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                repo.Delete(menu);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _publishEndpoint.Publish(
                    new MenuDeletedEvent { Id = request.Id },
                    cancellationToken);

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
