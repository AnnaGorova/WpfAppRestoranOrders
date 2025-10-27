using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppRestoranOrder.Models
{
    public class TopDish
    {
        public int Position { get; set; }
        public string Name { get; set; }
        public int OrderCount { get; set; }
        public int MenuItemId { get; set; }
    }
}
