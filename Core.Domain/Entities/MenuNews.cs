using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class MenuNews
    {
        public int MenuId { get; set; }
        public Menu Menu { get; set; }
        public int NewsId { get; set; }
        public News News { get; set; }

    }
}
