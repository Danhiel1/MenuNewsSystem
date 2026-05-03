using Core.Application.Events;
using Core.Application.ReadModels;
using MassTransit;
using MongoDB.Driver;

namespace Core.Application.Consumers
{
    public class NewsUpdatedConsumer : IConsumer<NewsUpdatedEvent>
    {
        private readonly IMongoCollection<NewsReadModel> _newsCollection;

        public NewsUpdatedConsumer(IMongoDatabase mongoDatabase)
        {
            _newsCollection = mongoDatabase.GetCollection<NewsReadModel>("News");
        }

        public async Task Consume(ConsumeContext<NewsUpdatedEvent> context)
        {
            var message = context.Message;

            var filter = Builders<NewsReadModel>.Filter.Eq(x => x.Id, message.Id);
            var update = Builders<NewsReadModel>.Update
                .Set(x => x.Title, message.Title)
                .Set(x => x.Content, message.Content);

            await _newsCollection.UpdateOneAsync(filter, update);
        }
    }
}
