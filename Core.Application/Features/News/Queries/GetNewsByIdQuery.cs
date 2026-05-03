using Core.Application.DTOs;
using Core.Application.ReadModels;
using MediatR;
using MongoDB.Driver;

namespace Core.Application.Features.News.Queries
{
    public class GetNewsByIdQuery : IRequest<NewsDto?>
    {
        public int NewsId { get; set; }
    }

    public class GetNewsByIdQueryHandler : IRequestHandler<GetNewsByIdQuery, NewsDto?>
    {
        private readonly IMongoCollection<NewsReadModel> _newsCollection;

        public GetNewsByIdQueryHandler(IMongoDatabase mongoDatabase)
        {
            _newsCollection = mongoDatabase.GetCollection<NewsReadModel>("News");
        }

        public async Task<NewsDto?> Handle(GetNewsByIdQuery request, CancellationToken cancellationToken)
        {
            var news = await _newsCollection
                .Find(n => n.Id == request.NewsId)
                .FirstOrDefaultAsync(cancellationToken);

            if (news == null) return null;

            return new NewsDto
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content
            };
        }
    }
}
