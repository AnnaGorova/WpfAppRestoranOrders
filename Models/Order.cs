using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppRestoranOrder.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();


        public string CustomName { get; set; }
        public string PhoneNumber   { get; set; }
        public string DeliveryAddress { get; set; }

    }
}
