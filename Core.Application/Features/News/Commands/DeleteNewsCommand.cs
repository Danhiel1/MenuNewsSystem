using Core.Application.Events;
using Core.Application.Interfaces;
using MassTransit;
using MediatR;

namespace Core.Application.Features.News.Commands
{
    public class DeleteNewsCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteNewsCommandHandler : IRequestHandler<DeleteNewsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public DeleteNewsCommandHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<bool> Handle(DeleteNewsCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<Core.Domain.Entities.News>();
            var news = await repo.GetByIdAsync(request.Id);
            if (news == null) return false;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                repo.Delete(news);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _publishEndpoint.Publish(
                    new NewsDeletedEvent { Id = request.Id },
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
