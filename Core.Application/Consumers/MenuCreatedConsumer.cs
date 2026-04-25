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
            // 1. Mở thư ra đọc thông tin
            var message = context.Message;

            // 2. Chuyển đổi dữ liệu từ Thư sang định dạng ReadModel của MongoDB
            var newMenuReadModel = new MenuReadModel
            {
                Id = message.Id,
                Name = message.Name,
                Description = message.Description,
                NewsList = new List<NewsItem>() // Khởi tạo mảng rỗng chờ News sau này
            };
            // 3. Lưu thẳng vào MongoDB
            await _menuCollection.InsertOneAsync(newMenuReadModel);
        }
    }
}
