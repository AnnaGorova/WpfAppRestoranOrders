using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppRestoranOrder.Models
{
    public enum OrderStatus
    {
        New,
        InProgress,
        Ready,
        Completed,    // видано замовнику
        Cancelled
    }
}
