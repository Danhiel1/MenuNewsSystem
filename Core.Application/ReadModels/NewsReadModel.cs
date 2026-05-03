using MongoDB.Bson.Serialization.Attributes;

namespace Core.Application.ReadModels
{
    public class NewsReadModel
    {
        [BsonId]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
