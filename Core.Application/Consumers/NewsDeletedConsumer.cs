using Core.Application.Events;
using Core.Application.ReadModels;
using MassTransit;
using MongoDB.Driver;

namespace Core.Application.Consumers
{
    public class NewsDeletedConsumer : IConsumer<NewsDeletedEvent>
    {
        private readonly IMongoCollection<NewsReadModel> _newsCollection;

        public NewsDeletedConsumer(IMongoDatabase mongoDatabase)
        {
            _newsCollection = mongoDatabase.GetCollection<NewsReadModel>("News");
        }

        public async Task Consume(ConsumeContext<NewsDeletedEvent> context)
        {
            var filter = Builders<NewsReadModel>.Filter.Eq(x => x.Id, context.Message.Id);
            await _newsCollection.DeleteOneAsync(filter);
        }
    }
}
