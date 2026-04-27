using Core.Application.Events;
using Core.Application.ReadModels;
using MassTransit;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Consumers
{
     public class MenuUpdatedConsumer : IConsumer<MenuUpdatedEvent>
    {
        private readonly IMongoCollection<MenuReadModel> _menuCollection;
        public MenuUpdatedConsumer(IMongoDatabase mongoDatabase)
        {
            _menuCollection = mongoDatabase.GetCollection<MenuReadModel>("Menus");        
        }

        public async Task Consume(ConsumeContext<MenuUpdatedEvent> context)
        {
            var message = context.Message;

         
            var filter = Builders<MenuReadModel>.Filter.Eq(x => x.Id, message.Id);

           
            var update = Builders<MenuReadModel>.Update
                .Set(x => x.Name, message.Name)
                .Set(x => x.Description, message.Description);

         
            await _menuCollection.UpdateOneAsync(filter, update);
        }
    }
}
