using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAppRestoranOrder.Models;
using WpfAppRestoranOrder.Services;
using WpfAppRestoranOrder.Models;
using System.Windows.Threading;

namespace WpfAppRestoranOrder.View.Kitchen
{
    /// <summary>
    /// Interaction logic for KitchenWindow.xaml
    /// </summary>
    public partial class KitchenWindow : Window
    {
        private DataService _dataService;
        private List<Order> _allOrders;
        private DispatcherTimer _refreshTimer;
        private SimpleChat _chat;
        public KitchenWindow(DataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;

            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(2);
            _refreshTimer.Tick += RefreshTimer_Tick;

            _dataService.RefreshData();

            _chat = new SimpleChat("КУХНЯ", false);
            _chat.OnMessageReceived = (msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    ChatBox.Text += $"{msg}\n";
                    ChatBox.ScrollToEnd(); 
                });
            };

            Loaded += KitchenWindow_Loaded;
            Closed += KitchenWindow_Closed;

        }

        private async void SendToAdminBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
              
                string message = MessageTextBox.Text.Trim();

               
                if (string.IsNullOrEmpty(message) || message == "Введіть повідомлення для адміна...")
                {
                    MessageBox.Show("Введіть текст повідомлення!", "Попередження");
                    return;
                }

                
                if (_chat == null)
                {
                    _chat = new SimpleChat("КУХНЯ", false);
                    _chat.StartListening();
                }

                
                ChatBox.Text += $"[КУХНЯ]: {message}\n";
                ChatBox.ScrollToEnd();

              
                await _chat.SendMessage(message);

                
                MessageTextBox.Text = "";

                
               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відправки: {ex.Message}", "Помилка");
            }
        }



        private void KitchenWindow_Closed(object sender, EventArgs e)
        {
            _refreshTimer.Stop();
            _chat?.StopListening(); 
            _chat?.Dispose(); 
        }

        private void KitchenWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadOrders();
            _refreshTimer.Start();

            
            _chat.StartListening();
            ChatBox.Text += "💬 Чат з адміном активовано...\n";
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadOrders(); 
        }
        
        private void LoadOrders()
        
        {
            _dataService.RefreshData();
            _allOrders = _dataService.Orders;

          
            var activeOrders = _allOrders
                .Where(order => order.Status != OrderStatus.Completed &&
                                order.Status != OrderStatus.Cancelled)
                .ToList();

            var kitchenOrders = activeOrders.Select(order => new KitchenOrderViewModel(order, _dataService)).ToList();
            ApplyStatusFilter(kitchenOrders);
            UpdateStatistics(); 
        }

        private void ApplyStatusFilter(List<KitchenOrderViewModel> orders)
        {
            var selectedFilter = (StatusFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            var filteredOrders = selectedFilter switch
            {
                "Нові" => orders.Where(o => o.Status == OrderStatus.New).ToList(),
                "В роботі" => orders.Where(o => o.Status == OrderStatus.InProgress).ToList(),
                "Готові" => orders.Where(o => o.Status == OrderStatus.Ready).ToList(),
                "Історія замовлень" => _allOrders 
                    .Where(order => order.Status == OrderStatus.Completed ||
                                   order.Status == OrderStatus.Cancelled)
                    .Select(order => new KitchenOrderViewModel(order, _dataService))
                    .ToList(),
                _ => orders 
            };
          

            OrdersItemsControl.ItemsSource = filteredOrders;
        }

        private void UpdateStatistics()
        {
            var newOrders = _allOrders.Count(o => o.Status == OrderStatus.New);
            var inProgress = _allOrders.Count(o => o.Status == OrderStatus.InProgress);
            var ready = _allOrders.Count(o => o.Status == OrderStatus.Ready);
            var active = newOrders + inProgress;

            var completed = _allOrders.Count(o => o.Status == OrderStatus.Completed);
            var cancelled = _allOrders.Count(o => o.Status == OrderStatus.Cancelled);

            NewOrdersCount.Text = newOrders.ToString();
            InProgressCount.Text = inProgress.ToString();
            ReadyCount.Text = ready.ToString();
            ActiveOrdersText.Text = active.ToString();
            //CompletedCount.Text = completed.ToString();
            //CancelledCount.Text = cancelled.ToString();
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_allOrders != null)
            {
                var kitchenOrders = _allOrders.Select(order => new KitchenOrderViewModel(order, _dataService)).ToList();
                ApplyStatusFilter(kitchenOrders);
            }
        }

        private void AcceptOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                UpdateOrderStatus(orderId, OrderStatus.InProgress);
            }
        }

        private void StartCookingBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                UpdateOrderStatus(orderId, OrderStatus.InProgress);
            }
        }

        private void ReadyOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                UpdateOrderStatus(orderId, OrderStatus.Ready);
            }
        }


        private void CompleteOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                var result = MessageBox.Show(
                    $"Позначити замовлення №{orderId} як 'Видано клієнту'?",
                    "Підтвердження",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    UpdateOrderStatus(orderId, OrderStatus.Completed);
                    MessageBox.Show($"Замовлення №{orderId} видано клієнту!", "Успіх");
                }
            }
        }

        private void CancelOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                var order = _allOrders.FirstOrDefault(o => o.Id == orderId);
                if (order != null)
                {
                    var result = MessageBox.Show(
                        $"Скасувати замовлення №{orderId} від {order.CustomerName}?\n\n",
                        "Скасування замовлення",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        
                        UpdateOrderStatus(orderId, OrderStatus.Cancelled);
                        MessageBox.Show($"Замовлення №{orderId} скасовано!", "Інформація");
                    }
                }
            }
        }


        private void UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = _allOrders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = newStatus;
                _dataService.UpdateOrderStatus(order);
                LoadOrders(); 
            }
        }
    }
}