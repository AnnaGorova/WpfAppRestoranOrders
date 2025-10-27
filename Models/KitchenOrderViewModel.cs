using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using WpfAppRestoranOrder.Services;

namespace WpfAppRestoranOrder.Models
{
    public class KitchenOrderViewModel
    {
        private readonly DataService _dataService;
        public Order Order { get; }

        public int Id => Order.Id;
        public DateTime OrderDate => Order.OrderDate;
        public OrderStatus Status => Order.Status;
        public decimal TotalAmount => Order.TotalAmount;
        public string CustomerName => Order.CustomerName;
        public string PhoneNumber => Order.PhoneNumber;
        public string DeliveryAddress => Order.DeliveryAddress;

        public string StatusDisplay => Status switch
        {
            OrderStatus.New => "НОВЕ",
            OrderStatus.InProgress => "В РОБОТІ",
            OrderStatus.Ready => "ГОТОВЕ",
            OrderStatus.Completed => "ВИДАНО",
            OrderStatus.Cancelled => "СКАСОВАНО",
            _ => Status.ToString()
        };

        public Brush StatusColor => Status switch
        {
            OrderStatus.New => new SolidColorBrush(Color.FromRgb(139, 0, 0)),    // Темно-червоний
            OrderStatus.InProgress => new SolidColorBrush(Color.FromRgb(255, 140, 0)), // Помаранчевий
            OrderStatus.Ready => new SolidColorBrush(Color.FromRgb(50, 205, 50)), // Зелений
            OrderStatus.Completed => new SolidColorBrush(Color.FromRgb(70, 130, 180)), // Синій
            OrderStatus.Cancelled => new SolidColorBrush(Color.FromRgb(128, 128, 128)), // Сірий
            _ => Brushes.White
        };

        public List<OrderItemViewModel> OrderItems { get; }

        // Видимість кнопок в залежності від статусу
        public Visibility AcceptButtonVisibility => Status == OrderStatus.New ? Visibility.Visible : Visibility.Collapsed;
        public Visibility CookingButtonVisibility => Status == OrderStatus.InProgress ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ReadyButtonVisibility => Status == OrderStatus.InProgress ? Visibility.Visible : Visibility.Collapsed;
        public Visibility DeliveryAddressVisibility => !string.IsNullOrEmpty(DeliveryAddress) ? Visibility.Visible : Visibility.Collapsed;
        public Visibility CompleteButtonVisibility =>
             Status == OrderStatus.Ready ? Visibility.Visible : Visibility.Collapsed;

        public Visibility CancelButtonVisibility =>
            (Status == OrderStatus.New || Status == OrderStatus.InProgress) ?
            Visibility.Visible : Visibility.Collapsed;

        public KitchenOrderViewModel(Order order, DataService dataService)
        {
            Order = order;
            _dataService = dataService;
            OrderItems = GetOrderItems();
        }

        private List<OrderItemViewModel> GetOrderItems()
        {
            var orderItems = _dataService.GetOrderItemsByOrderId(Order.Id);
            var menuItems = _dataService.MenuItems;

            return orderItems.Select(oi => new OrderItemViewModel
            {
                Name = menuItems.First(m => m.Id == oi.MenuItemId).Name,
                Quantity = oi.Quantity
            }).ToList();
        }
    }
}
