using Core.Application.Events;
using Core.Application.ReadModels;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;


namespace Core.Application.Consumer
{
    public class MenuCreatedConsumer : IConsumer<MenuCreatedEvent>
    {
        private readonly IMongoCollection<MenuReadModel> _menuCollection;

        public MenuCreatedConsumer(IMongoDatabase mongoDatabase)
        {
            _menuCollection = mongoDatabase.GetCollection<MenuReadModel>("Menus");
        }
        public async Task Consume(ConsumeContext<MenuCreatedEvent> context)
        {
           
            var message = context.Message;

           
            var newMenuReadModel = new MenuReadModel
            {
                Id = message.Id,
                Name = message.Name,
                Description = message.Description,
                NewsList = new List<NewsItem>() // Khởi tạo mảng rỗng chờ News sau này
            };
                     await _menuCollection.InsertOneAsync(newMenuReadModel);
        }
    }
}
