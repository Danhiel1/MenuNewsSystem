using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.DTOs
{
    public class MenuDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
         // Danh sách các bài News nằm trong Menu này
        public List<NewsDto> NewsList { get; set; } = new List<NewsDto>();
    }
}
