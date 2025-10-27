using System;
using System.Windows;
using System.Windows.Controls;
using WpfAppRestoranOrder.Models;
using WpfAppRestoranOrder.Services;
using Microsoft.VisualBasic;

namespace WpfAppRestoranOrder.Admin
{
    public partial class AdminWindow : Window
    {
        private DataService _dataService;
        private SimpleChat _chat;
        public AdminWindow()
        {
            InitializeComponent();
            _dataService = new DataService();

            _chat = new SimpleChat("АДМІН", true);
            _chat.OnMessageReceived = (msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    ChatBox.Text += $"{msg}\n";
                    ChatBox.ScrollToEnd(); 
                });
            };

            Loaded += AdminWindow_Loaded;
            Closed += AdminWindow_Closed;
        }

        public AdminWindow(DataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;
            _chat = new SimpleChat("АДМІН", true);
            _chat.OnMessageReceived = (msg) =>
            {
                Dispatcher.Invoke(() =>
                {
                    ChatBox.Text += $"{msg}\n";
                    ChatBox.ScrollToEnd();
                });
            };

            Loaded += AdminWindow_Loaded;
            Closed += AdminWindow_Closed;

        }

        private void AdminWindow_Closed(object sender, EventArgs e)
        {
            _chat?.StopListening();
            _chat?.Dispose();
        }

        private async void SendToKitchenBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                string message = MessageTextBox.Text.Trim();

              
                if (string.IsNullOrEmpty(message) || message == "Введіть повідомлення для кухні...")
                {
                    MessageBox.Show("Введіть текст повідомлення!", "Попередження");
                    return;
                }

              
                if (_chat == null)
                {
                    _chat = new SimpleChat("АДМІН", true);
                    _chat.StartListening();
                }

                
                ChatBox.Text += $"[АДМІН]: {message}\n";
                ChatBox.ScrollToEnd();

                
                await _chat.SendMessage(message);

                
                MessageTextBox.Text = "";

                
               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відправки: {ex.Message}", "Помилка");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _chat?.StopListening(); 
            _chat?.Dispose(); 
            base.OnClosed(e);
        }



        private void AdminWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _dataService.LoadAllData();
                LoadAllTablesData();

               
                if (_chat != null)
                {
                    _chat.StartListening();
                    ChatBox.Text += "💬 Чат з кухнею активовано...\n";
                }
                else
                {
                    ChatBox.Text += "❌ Чат не ініціалізовано\n";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}", "Помилка");
            }
        }
        private void LoadAllTablesData()
        {
            
            AllCategoriesGrid.ItemsSource = _dataService.Categories;

            
            AllMenuGrid.ItemsSource = _dataService.MenuItems;

           
            AllOrdersGrid.ItemsSource = _dataService.Orders;
                
            var orderItems = _dataService.GetOrderItems();
            AllOrderItemsGrid.ItemsSource = orderItems;

          
            UpdateTablesStatistics();
        }

        

        // 📂 КАТЕГОРІЇ - CRUD
        private void AddCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            var name = Interaction.InputBox("Ввведіть назву категорії: ");

            if (string.IsNullOrEmpty(name)) return;

            
            var imageUrl = Interaction.InputBox("Введіть шлях до зображаення або залиште поле пустим: ");


            var success = _dataService.AddCategory(new Category { Name = name, ImageUrl = imageUrl });
            if (success)
            {
                _dataService.RefreshData();
                AllCategoriesGrid.ItemsSource = _dataService.Categories;
                MessageBox.Show($"Категорію {name} успішно додано!");
            }
            else
            {
                MessageBox.Show($"Error");
            }
        }

        private void EditCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AllCategoriesGrid.SelectedItem is Category selectedCategory)
            {
                string newName = Interaction.InputBox(
                    "Введіть нову назву категорії: ", "Редагувати категорію",
                    selectedCategory.Name);
                if (string.IsNullOrEmpty(newName)) return;

                string newImageUrl = Interaction.InputBox("Введіть шлях до зображення: ", 
                    selectedCategory.ImageUrl);

                var updateCategory = new Category
                {
                    Id = selectedCategory.Id,
                    Name = newName,
                    ImageUrl = newImageUrl?.Trim() ?? ""
                };

                bool success = _dataService.UpdateCategory(updateCategory);

                if (success) 
                    {
                        _dataService.RefreshData();
                        AllCategoriesGrid.ItemsSource = _dataService.Categories;
                        MessageBox.Show($"Категорію {newName} успішно оновлено!");
                    }
                    else
                    {
                        MessageBox.Show("Помилка при оновленні категорії");
                    }
                   
            }
            else
            {
                MessageBox.Show("Виберіть категорію для редагування");
            }
        }

        private void DeleteCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AllCategoriesGrid.SelectedItem is Category selectedCategory)
            {
                
                var result = MessageBox.Show(
                    $"Ви впевнені, що хочете видалити категорію '{selectedCategory.Name}'?",
                    "Підтвердження видалення",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                
                if (result == MessageBoxResult.Yes)
                {
                    
                    bool success = _dataService.DeleteCategory(selectedCategory.Id);

                    if (success)
                    {
                        
                        _dataService.RefreshData();
                        AllCategoriesGrid.ItemsSource = _dataService.Categories;
                        MessageBox.Show($"Категорію '{selectedCategory.Name}' успішно видалено!", "Успіх");
                    }
                    else
                    {
                        MessageBox.Show("Не вдалося видалити категорію", "Помилка");
                    }
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть категорію для видалення", "Попередження");
            }
        }

        private void RefreshCategoriesBtn_Click(object sender, RoutedEventArgs e)
        {
            _dataService.RefreshData();
            AllCategoriesGrid.ItemsSource = _dataService.Categories;
        }


        // 🍽️ МЕНЮ - CRUD
        private void AddMenuItemBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = Interaction.InputBox("Введіть назву страви: ");
            if (string.IsNullOrEmpty(name)) return;

            string description = Interaction.InputBox("Введіть опис страви:  ");

            string priceInput = Interaction.InputBox("Введіть ціну страви: ");
            if (!decimal.TryParse(priceInput, out decimal price))
            {
                MessageBox.Show("Введіть корректну ціну!");
                return;
            }

            string category = Interaction.InputBox("Ведіть назву категорії");
            if (string.IsNullOrEmpty(category)) return;

            string imageUrl = Interaction.InputBox("Введіть шлях до зображення або залиште поле пустим: ");

            var availabilityResult = MessageBox.Show("Страва доступна для замовлення?", "Доступність",
                  MessageBoxButton.YesNo, MessageBoxImage.Question);
            bool isAvailable = availabilityResult == MessageBoxResult.Yes;


            var newMenuItem = new Models.MenuItem
            {
                Name = name,
                Description = description ?? "",
                Price = price,
                Category = category,
                IsAvailable = isAvailable,
                ImageUrl = imageUrl ?? ""
            };


            bool success = _dataService.AddMenuItem(newMenuItem);
            if (success)
            {
                MessageBox.Show($"Страву {name} успішно додано: {price} грн\nКатегорія -> {category}");
                RefreshMenu();
            }
            else
            {
                MessageBox.Show("Виникла помилка при додаванні страви");            }

           

        }


        private void EditMenuItemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AllMenuGrid.SelectedItem is Models.MenuItem selectedItem)
            {
                string newName = Interaction.InputBox("Введіть нову назву страви:", "Редагувати страву", selectedItem.Name);
                if (string.IsNullOrEmpty(newName)) return;

                string newDescription = Interaction.InputBox("Введіть новий опис страви:", "Опис страви", selectedItem.Description);

                string newPriceInput = Interaction.InputBox("Введіть нову ціну страви:", "Ціна страви", selectedItem.Price.ToString());
                if (!decimal.TryParse(newPriceInput, out decimal newPrice))
                {
                    MessageBox.Show("Введіть коректну ціну!", "Помилка");
                    return;
                }

                string newCategory = Interaction.InputBox("Введіть нову категорію:", "Категорія страви", selectedItem.Category);
                if (string.IsNullOrEmpty(newCategory)) return;

                string newImageUrl = Interaction.InputBox("Введіть новий шлях до зображення:", "Зображення страви", selectedItem.ImageUrl);

                
                var availabilityResult = MessageBox.Show("Страва доступна для замовлення?", "Доступність",
                    MessageBoxButton.YesNo,
                    selectedItem.IsAvailable ? MessageBoxImage.Question : MessageBoxImage.Warning);
                bool newIsAvailable = availabilityResult == MessageBoxResult.Yes;


                var updatedMenuItem = new Models.MenuItem
                {
                    Id = selectedItem.Id,
                    Name = newName,
                    Description = newDescription ?? "",
                    Price = newPrice,
                    Category = newCategory,
                    IsAvailable = newIsAvailable,
                    ImageUrl = newImageUrl ?? ""
                };

                bool success = _dataService.UpdateMenuItem(updatedMenuItem);
                if (success)
                {
                    MessageBox.Show($"Страву '{newName}' успішно оновлено!", "Успіх");
                    RefreshMenu();
                }
                else
                {
                    MessageBox.Show("Помилка при оновленні страви", "Помилка");
                }
            }
            else
            {
                MessageBox.Show("Виберіть страву для редагування", "Попередження");
            }
        }

        private void DeleteMenuItemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AllMenuGrid.SelectedItem is Models.MenuItem selectedItem)
            {
                
                var orderItems = _dataService.GetOrderItems();
                bool isUsed = orderItems.Any(oi => oi.MenuItemId == selectedItem.Id);

                string message;
                MessageBoxImage icon;

                if (isUsed)
                {
                    message = $"Страву '{selectedItem.Name}' використовували в замовленнях.\n\n" +
                             "⚠️ Вона буде прихована з меню, але залишиться в базі даних для історії замовлень.";
                    icon = MessageBoxImage.Information;
                }
                else
                {
                    message = $"Видалити страву '{selectedItem.Name}' повністю з бази даних?\n\n" +
                             "🗑️ Ця дія незворотня!";
                    icon = MessageBoxImage.Warning;
                }

                var result = MessageBox.Show(message, "Видалення страви",
                    MessageBoxButton.YesNo, icon);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = _dataService.DeleteMenuItem(selectedItem.Id);

                    if (success)
                    {
                        RefreshMenu();
                        if (isUsed)
                        {
                            MessageBox.Show($"Страву '{selectedItem.Name}' приховано!", "Успіх");
                        }
                        else
                        {
                            MessageBox.Show($"Страву '{selectedItem.Name}' повністю видалено!", "Успіх");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Помилка при видаленні страви", "Помилка");
                    }
                }
            }
            else
            {
                MessageBox.Show("Виберіть страву для видалення");
            }
        }
        private void RefreshMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshMenu();
        }
        private void RefreshMenu()
        {
            _dataService.RefreshData();

            AllMenuGrid.ItemsSource = _dataService.MenuItems;
            UpdateTablesStatistics();
        }


        // 📋 ЗАМОВЛЕННЯ - CRUD
        private void EditOrderContactBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AllOrdersGrid.SelectedItem is Order selectedOrder)
            {
                
                if (selectedOrder.Status == OrderStatus.New || selectedOrder.Status == OrderStatus.InProgress)
                {
                    string newName = Interaction.InputBox("Ім'я клієнта:", 
                        "Редагування контактів", selectedOrder.CustomerName);
                    if (string.IsNullOrEmpty(newName)) return;

                    string newPhone = Interaction.InputBox("Телефон:",
                        "Редагування контактів", selectedOrder.PhoneNumber);
                    if (string.IsNullOrEmpty(newPhone)) return;

                    
                    string newAddress = Interaction.InputBox("Адреса доставки:", 
                        "Редагування контактів", selectedOrder.DeliveryAddress);


                    var updatedOrder = new Order
                    {
                        Id = selectedOrder.Id,
                        CustomerName = newName,
                        PhoneNumber = newPhone,
                        DeliveryAddress = newAddress,
                        OrderDate = selectedOrder.OrderDate,
                        Status = selectedOrder.Status,
                        TotalAmount = selectedOrder.TotalAmount
                    };

                    bool success = _dataService.UpdateOrderContacts(updatedOrder);
                    if (success)
                    {
                        _dataService.RefreshData();
                        AllOrdersGrid.ItemsSource = _dataService.Orders;
                        UpdateTablesStatistics();
                        MessageBox.Show("Контактні дані замовлення оновлено!", "Успіх");
                    }
                    else
                    {
                        MessageBox.Show("Помилка при оновленні контактних даних", "Помилка");
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Контактні дані можна редагувати тільки для замовлень зі статусом 'New' або 'InProgress'",
                        "Обмеження",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
            }
            else
            {
                MessageBox.Show("Виберіть замовлення для редагування контактів", "Попередження");
            }
        }
    
       

        private void EditOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AllOrdersGrid.SelectedItem is Order selectedOrder)
            {
                string statusInput = Interaction.InputBox(
                    "Змінити статус замовлення: \n\n" +
                    "1 - Нове\n" +
                    "2 - В роботі\n" +
                    "3 - Готове\n" +
                    "4 - Видане\n" +
                    "5 - Скасоване",
                    "Статус замовлення",
                    selectedOrder.Status.ToString()

                    );
                if (Enum.TryParse<OrderStatus>(statusInput, out OrderStatus newStatus))
                {
                    var updateOrder = new Order
                    {
                        Id = selectedOrder.Id,
                        OrderDate = selectedOrder.OrderDate,
                        Status = newStatus,
                        TotalAmount = selectedOrder.TotalAmount,
                        CustomerName = selectedOrder.CustomerName,
                        PhoneNumber = selectedOrder.PhoneNumber,
                        DeliveryAddress = selectedOrder.DeliveryAddress,
                    };

                    bool success = _dataService.UpdateOrderStatus(updateOrder);
                    if (success)
                    {
                        _dataService.RefreshData();
                        AllOrdersGrid.ItemsSource = _dataService.Orders;
                        UpdateTablesStatistics();
                        MessageBox.Show($"Статус замовлення №{selectedOrder.Id} змінено на {newStatus}");
                    }
                    else
                    {
                        MessageBox.Show("Помилка при оновленні статусу");
                    }
                }
            }
            else
            {
                MessageBox.Show("Виберіть замовлення для редагування статусу");
            }
        }



        private void DeleteOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AllOrdersGrid.SelectedItem is Order selectedOrder)
            {
                if (selectedOrder.Status == OrderStatus.New)
                {
                    var result = MessageBox.Show($"Видалити нове замовлення " +
                        $"№{selectedOrder.Id} від {selectedOrder.CustomerName} - {selectedOrder.PhoneNumber}?",
                        "Підтвердження видалення",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                        );

                    if (result == MessageBoxResult.Yes)
                    {
                        bool success = _dataService.DeleteOrder(selectedOrder.Id);
                        if (success)
                        {
                            RefreshOrders();
                            MessageBox.Show($"Замовлення видалено №{selectedOrder.Id} від {selectedOrder.CustomerName} - {selectedOrder.PhoneNumber}");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Виберіть замовлення для видалення. Можна видалити замовлення лише зі статусом => New. " +
                        "Для інших статусів використовуйте статус 'Cancelled'", "Обмеження",
                        MessageBoxButton.OK, MessageBoxImage.Warning
                    );

                }
            }else
            {
                MessageBox.Show("Виберіть замовлення для видалення");
            }
        }

        private void RefreshOrdersBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshOrders();
        }
        private void RefreshOrders()
        {
            _dataService.RefreshData();
            AllOrdersGrid.ItemsSource = _dataService.Orders;
            UpdateTablesStatistics();
        }

        // 🛒 ПОЗИЦІЇ - CRUD


        private void RefreshOrderItemsBtn_Click(object sender, RoutedEventArgs e)
        {
            _dataService.RefreshData();
            var orderItems = _dataService.GetOrderItems();
            AllOrderItemsGrid.ItemsSource = orderItems;
        }



        // Статистика

        private void UpdateTablesStatistics()
        {
            CategoriesCount.Text = (_dataService.Categories?.Count ?? 0).ToString();
            MenuItemsCount.Text = (_dataService.MenuItems?.Count ?? 0).ToString();
            OrdersCount.Text = (_dataService.Orders?.Count ?? 0).ToString();

            var orderItems = _dataService.GetOrderItems();
            OrderItemsCount.Text = (orderItems?.Count ?? 0).ToString();

            CalculateRevenueStatistics();

            UpdateTopDishesStatistics();
        }


        private void RefreshAllDataBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _dataService.RefreshData();
                LoadAllTablesData();
                MessageBox.Show("Дані оновлено!", "Інформація");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка оновлення: {ex.Message}", "Помилка");
            }
        }


             

        private void CalculateRevenueStatistics()
        {
            if (_dataService.Orders == null) return;

            
            var completedOrders = _dataService.Orders
                .Where(order => order.Status == OrderStatus.Completed)
                .ToList();

            
            var cancelledOrders = _dataService.Orders
                .Where(order => order.Status == OrderStatus.Cancelled)
                .ToList();

            
            var allOrders = _dataService.Orders.ToList();

            
            decimal totalRevenue = completedOrders.Sum(order => order.TotalAmount);
            decimal cancelledAmount = cancelledOrders.Sum(order => order.TotalAmount);
            decimal potentialRevenue = allOrders.Sum(order => order.TotalAmount);

           
            TotalRevenueText.Text = $"{totalRevenue:0.00} ₴";
            CompletedOrdersCount.Text = completedOrders.Count.ToString();
            CancelledOrdersCount.Text = cancelledOrders.Count.ToString();

           
            UpdateAdditionalStatistics(completedOrders, cancelledOrders, totalRevenue, cancelledAmount, potentialRevenue);
        }

        private void UpdateAdditionalStatistics(List<Order> completedOrders, List<Order> cancelledOrders,
                                      decimal totalRevenue, decimal cancelledAmount, decimal potentialRevenue)
        {
            double successRate = completedOrders.Count > 0 ?
                (double)completedOrders.Count / (completedOrders.Count + cancelledOrders.Count) * 100 : 0;

            
            string statisticsText =
                $"💰 Загальний дохід: {totalRevenue:0.00} ₴\n" +
                $"❌ Втрачено через скасування: {cancelledAmount:0.00} ₴\n" +
                $"📈 Потенційний дохід: {potentialRevenue:0.00} ₴\n" +
                $"✅ Успішних замовлень: {completedOrders.Count}\n" +
                $"🚫 Скасованих замовлень: {cancelledOrders.Count}\n" +
                $"🎯 Успішність: {successRate:0.0}%";

            if (DetailedStatsText != null)
                DetailedStatsText.Text = statisticsText;
        }





        private List<TopDish> GetTopDishes(int topCount = 5)
        {
            var orderItems = _dataService.GetOrderItems();
            var menuItems = _dataService.MenuItems;

            
            var dishStats = orderItems
                .GroupBy(oi => oi.MenuItemId)
                .Select(g => new
                {
                    MenuItemId = g.Key,
                    TotalQuantity = g.Sum(oi => oi.Quantity),
                    OrderCount = g.Count()
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(topCount)
                .ToList();

            
            var topDishes = new List<TopDish>();
            int position = 1;

            foreach (var dish in dishStats)
            {
                var menuItem = menuItems.FirstOrDefault(m => m.Id == dish.MenuItemId);
                if (menuItem != null)
                {
                    topDishes.Add(new TopDish
                    {
                        Position = position++,
                        Name = menuItem.Name,
                        OrderCount = dish.TotalQuantity,
                        MenuItemId = dish.MenuItemId
                    });
                }
            }

            return topDishes;
        }



        private void UpdateTopDishesStatistics()
        {
            var topDishes = GetTopDishes(5);
            TopDishesItemsControl.ItemsSource = topDishes;
        }
    }
}