using Core.Application.Events;
using Core.Application.Interfaces;
using MassTransit;
using MediatR;

namespace Core.Application.Features.News.Commands
{
    public class UpdateNewsCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class UpdateNewsCommandHandler : IRequestHandler<UpdateNewsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public UpdateNewsCommandHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<bool> Handle(UpdateNewsCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<Core.Domain.Entities.News>();
            var news = await repo.GetByIdAsync(request.Id);
            if (news == null) return false;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                news.Title = request.Title;
                news.Content = request.Content;

                repo.Update(news);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _publishEndpoint.Publish(new NewsUpdatedEvent
                {
                    Id = request.Id,
                    Title = request.Title,
                    Content = request.Content
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
