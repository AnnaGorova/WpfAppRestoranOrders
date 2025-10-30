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
using System.Windows.Threading;
using WpfAppRestoranOrder.Models;
using WpfAppRestoranOrder.Services;

namespace WpfAppRestoranOrder.View.Client
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        public event Action DataChanged;

        private DataService _dataService;
        private List<Models.MenuItem> _currentMenuItems;  
        private List<Models.CartItem> _cartItems;
        public List<Models.Category> Categories { get; set; }
        public List<Models.Order> Orders { get; set; }
        private DispatcherTimer _refreshTimer;


        public ClientWindow(DataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;
            _cartItems = new List<Models.CartItem>();


            _dataService.RefreshData();


            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(2); 
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();

            LoadData();
          
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
           
            RefreshDataSilently();
        }

        private void RefreshDataSilently()
        {
           
            var selectedCategory = CategoriesListBox.SelectedItem as Category;

            
            _dataService.RefreshData();

           
            CategoriesListBox.ItemsSource = _dataService.Categories;

            
            if (selectedCategory != null)
            {
                var categoryToSelect = _dataService.Categories.FirstOrDefault(c => c.Name == selectedCategory.Name);
                CategoriesListBox.SelectedItem = categoryToSelect;
            }

            UpdateCartDisplay();
        }

        private void UpdateData()
        {
            _dataService.RefreshData();
            
            CategoriesListBox.ItemsSource = _dataService.Categories;

           
            
            UpdateCartDisplay();
        }

       

        private void LoadData()
        {
         
            
            CategoriesListBox.ItemsSource = _dataService.Categories;

            
            MenuItemsControl.ItemsSource = _dataService.MenuItems
                .Where(m => m.IsAvailable)
                .ToList();
           

            UpdateCartDisplay();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _refreshTimer.Stop();
        }


        private void CategoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoriesListBox.SelectedItem is Category selectedCategory)
            {
                var filteredItems = _dataService.MenuItems
                    .Where(item => item.Category == selectedCategory.Name && item.IsAvailable)
                    .ToList();
                MenuItemsControl.ItemsSource = filteredItems;
            }
            else
            {
                MenuItemsControl.ItemsSource = _dataService.MenuItems
                    .Where(m => m.IsAvailable)
                    .ToList();
            }
        }

        private void AllDishesButton_Click(object sender, RoutedEventArgs e)
        {
            
            CategoriesListBox.SelectedItem = null;

           
            MenuItemsControl.ItemsSource = _dataService.MenuItems
                .Where(m => m.IsAvailable)
                .ToList();
        }
        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int menuItemId)
            {
                
                var menuItem = _dataService.MenuItems.FirstOrDefault(m => m.Id == menuItemId);
                if (menuItem != null && menuItem.IsAvailable)
                {
                    var existingItem = _cartItems.FirstOrDefault(item => item.MenuItemId == menuItemId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity++;
                    }
                    else
                    {
                        _cartItems.Add(new Models.CartItem
                        {
                            MenuItemId = menuItem.Id,
                            Name = menuItem.Name,
                            Price = menuItem.Price,
                            Quantity = 1
                        });
                    }

                    UpdateCartDisplay();
                }
            }
        }

        private void UpdateCartDisplay()
        {
            CartItemsControl.ItemsSource = _cartItems.ToList();
            CartCountText.Text = _cartItems.Sum(item => item.Quantity).ToString();

            var totalAmount = _cartItems.Sum(item => item.Price * item.Quantity).ToString("0.00");
            CartTotalHeaderText.Text = totalAmount;     
            CartTotalAmountText.Text = totalAmount;   
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_cartItems.Any())
            {
                MessageBox.Show("Кошик порожній!", "Увага", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            var customerWindow = new CustomerInfoWindow();
            if (customerWindow.ShowDialog() == true)
            {
                try
                {
                   
                    var newOrder = new Order
                    {
                        OrderDate = DateTime.Now,
                        Status = OrderStatus.New,
                        TotalAmount = _cartItems.Sum(item => item.Price * item.Quantity),
                        CustomerName = customerWindow.CustomerName,
                        PhoneNumber = customerWindow.PhoneNumber,
                        DeliveryAddress = customerWindow.DeliveryAddress,
                        Items = new List<Models.MenuItem>()
                    };

                    
                    if (_dataService.CreateOrder(newOrder, _cartItems))
                    {
                        MessageBox.Show($"Замовлення прийнято!\nСума: {newOrder.TotalAmount:0.00}₴\nСтатус: {newOrder.Status}",
                            "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                        _cartItems.Clear();
                        UpdateCartDisplay();
                    }
                    else
                    {
                        MessageBox.Show("Помилка при збереженні замовлення", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private void IncreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int menuItemId)
            {
                var cartItem = _cartItems.FirstOrDefault(item => item.MenuItemId == menuItemId);
                if (cartItem != null)
                {
                    cartItem.Quantity++;
                    UpdateCartDisplay();
                }
            }
        }

        private void DecreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int menuItemId)
            {
                var cartItem = _cartItems.FirstOrDefault(item => item.MenuItemId == menuItemId);
                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                    }
                    else
                    {
                        
                        _cartItems.Remove(cartItem);
                    }
                    UpdateCartDisplay();
                }
            }
        }

        private void RemoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int menuItemId)
            {
                var cartItem = _cartItems.FirstOrDefault(item => item.MenuItemId == menuItemId);
                if (cartItem != null)
                {
                    _cartItems.Remove(cartItem);
                    UpdateCartDisplay();
                }
            }
        }


    }
}