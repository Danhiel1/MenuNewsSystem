using Core.Application.DTOs;
using Core.Application.Interfaces;
using MediatR;
using MongoDB.Driver;
using Core.Application.ReadModels;

namespace Core.Application.Features.News.Queries
{
    public class GetAllNewsQuery : IRequest<IEnumerable<NewsDto>>
    {
    }

    public class GetAllNewsQueryHandler : IRequestHandler<GetAllNewsQuery, IEnumerable<NewsDto>>
    {
        private readonly IMongoCollection<NewsReadModel> _newsCollection;

        public GetAllNewsQueryHandler(IMongoDatabase mongoDatabase)
        {
            _newsCollection = mongoDatabase.GetCollection<NewsReadModel>("News");
        }

        public async Task<IEnumerable<NewsDto>> Handle(GetAllNewsQuery request, CancellationToken cancellationToken)
        {
            var newsList = await _newsCollection.Find(_ => true).ToListAsync(cancellationToken);

            return newsList.Select(n => new NewsDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content
            });
        }
    }
}
