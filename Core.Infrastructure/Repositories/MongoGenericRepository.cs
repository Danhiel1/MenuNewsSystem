using Core.Application.Interfaces;
using MongoDB.Driver;

namespace Core.Infrastructure.Repositories
{
    /// <summary>
    /// TẠI SAO tách MongoGenericRepository riêng?
    /// - Giống GenericRepository nhưng dùng MongoDB thay vì SQL Server
    /// - Đây là Write operations cho MongoDB (Insert, Update, Delete)
    /// - Được gọi bởi MassTransit Consumers khi nhận event từ RabbitMQ
    /// 
    /// Flow: Handler → SaveChanges SQL → Publish Event → RabbitMQ → Consumer → MongoGenericRepository
    /// </summary>
    public class MongoGenericRepository<T> : IGenericReadRepository<T> where T : class, IDocument
    {
        protected readonly IMongoCollection<T> _collection;

        public MongoGenericRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<T>(typeof(T).Name);
        }

        public async Task InsertAsync(T document, CancellationToken cancellationToken)
        {
            await _collection.InsertOneAsync(document, null, cancellationToken);
        }

        public async Task UpdateAsync(T document, CancellationToken cancellationToken)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, document.Id);
            await _collection.ReplaceOneAsync(filter, document, new ReplaceOptions { IsUpsert = false }, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await _collection.DeleteOneAsync(doc => doc.Id == id, cancellationToken);
        }
    }
}
