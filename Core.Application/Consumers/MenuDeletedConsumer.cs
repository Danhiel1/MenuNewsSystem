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
    public class MenuDeletedConsumer : IConsumer<MenuDeletedEvent>
    {
        private readonly IMongoCollection<MenuReadModel> _menuCollection;
        public MenuDeletedConsumer(IMongoDatabase mongoDatabase)
        {
            _menuCollection = mongoDatabase.GetCollection<MenuReadModel>("Menus");
        }

        public async Task Consume(ConsumeContext<MenuDeletedEvent> context)
        {
            var menuId = context.Message.Id;
            await _menuCollection.DeleteOneAsync(x=> x.Id == menuId);

        }
    }
}
