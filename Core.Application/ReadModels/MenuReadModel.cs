using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Application.ReadModels
{
    public class MenuReadModel
    {
        [BsonId]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<NewsItem> NewsList { get; set; } = new List<NewsItem>();
    }
    public class NewsItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
