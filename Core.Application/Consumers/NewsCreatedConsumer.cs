using Core.Application.Events;
using Core.Application.ReadModels;
using MassTransit;
using MongoDB.Driver;

namespace Core.Application.Consumers
{
    public class NewsCreatedConsumer : IConsumer<NewsCreatedEvent>
    {
        private readonly IMongoCollection<NewsReadModel> _newsCollection;

        public NewsCreatedConsumer(IMongoDatabase mongoDatabase)
        {
            _newsCollection = mongoDatabase.GetCollection<NewsReadModel>("News");
        }

        public async Task Consume(ConsumeContext<NewsCreatedEvent> context)
        {
            var message = context.Message;

            var newsReadModel = new NewsReadModel
            {
                Id = message.Id,
                Title = message.Title,
                Content = message.Content
            };

            await _newsCollection.InsertOneAsync(newsReadModel);
        }
    }
}
