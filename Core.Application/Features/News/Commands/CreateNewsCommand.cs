using Core.Application.Events;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using MassTransit;
using MediatR;

namespace Core.Application.Features.News.Commands
{
    public class CreateNewsCommand : IRequest<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class CreateNewsCommandHandler : IRequestHandler<CreateNewsCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateNewsCommandHandler(IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<int> Handle(CreateNewsCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var news = new Core.Domain.Entities.News
                {
                    Title = request.Title,
                    Content = request.Content
                };

                var repo = _unitOfWork.Repository<Core.Domain.Entities.News>();
                await repo.AddAsync(news);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _publishEndpoint.Publish(new NewsCreatedEvent
                {
                    Id = news.Id,
                    Title = news.Title,
                    Content = news.Content
                }, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);
                return news.Id;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
