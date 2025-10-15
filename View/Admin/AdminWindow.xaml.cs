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

        public AdminWindow()
        {
            InitializeComponent();
            _dataService = new DataService();
            Loaded += AdminWindow_Loaded;
        }

        public AdminWindow(DataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;
            Loaded += AdminWindow_Loaded;
        }

        private void AdminWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _dataService.LoadAllData();
                LoadAllTablesData();
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

        private void UpdateTablesStatistics()
        {
            CategoriesCount.Text = (_dataService.Categories?.Count ?? 0).ToString();
            MenuItemsCount.Text = (_dataService.MenuItems?.Count ?? 0).ToString();
            OrdersCount.Text = (_dataService.Orders?.Count ?? 0).ToString();

            var orderItems = _dataService.GetOrderItems();
            OrderItemsCount.Text = (orderItems?.Count ?? 0).ToString();
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
                var result = MessageBox.Show(
                    $"Ви впевнені, що хочете видалити страву '{selectedItem.Name}'?",
                    "Підтвердження видалення",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    bool success = _dataService.DeleteMenuItem(selectedItem.Id);

                    if (success)
                    {
                        
                        RefreshMenu();
                        MessageBox.Show($"Страва '{selectedItem.Name}' успішно видалена!", "Успіх");
                    }
                    else
                    {
                        MessageBox.Show("Помилка при видаленні страви");
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
        private void AddOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Додати замовлення - функція в розробці", "Інформація");
        }

        private void EditOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AllOrdersGrid.SelectedItem is Order selectedOrder)
            {
                MessageBox.Show($"Редагувати замовлення: #{selectedOrder.Id}", "Інформація");
            }
            else
            {
                MessageBox.Show("Виберіть замовлення для редагування", "Попередження");
            }
        }

        private void DeleteOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AllOrdersGrid.SelectedItem is Order selectedOrder)
            {
                var result = MessageBox.Show($"Видалити замовлення #{selectedOrder.Id}?",
                    "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show($"Замовлення #{selectedOrder.Id} видалено", "Інформація");
                }
            }
            else
            {
                MessageBox.Show("Виберіть замовлення для видалення", "Попередження");
            }
        }

        private void RefreshOrdersBtn_Click(object sender, RoutedEventArgs e)
        {
            _dataService.RefreshData();
            AllOrdersGrid.ItemsSource = _dataService.Orders;
        }
       

       // 🛒 ПОЗИЦІЇ - CRUD
        private void AddOrderItemBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Додати позицію - функція в розробці", "Інформація");
        }

        private void EditOrderItemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AllOrderItemsGrid.SelectedItem is OrderItem selectedItem)
            {
                MessageBox.Show($"Редагувати позицію: ID {selectedItem.Id}", "Інформація");
            }
            else
            {
                MessageBox.Show("Виберіть позицію для редагування", "Попередження");
            }
        }

        private void DeleteOrderItemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AllOrderItemsGrid.SelectedItem is OrderItem selectedItem)
            {
                var result = MessageBox.Show($"Видалити позицію ID {selectedItem.Id}?",
                    "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show($"Позиція ID {selectedItem.Id} видалена", "Інформація");
                }
            }
            else
            {
                MessageBox.Show("Виберіть позицію для видалення", "Попередження");
            }
        }

        private void RefreshOrderItemsBtn_Click(object sender, RoutedEventArgs e)
        {
            _dataService.RefreshData();
            var orderItems = _dataService.GetOrderItems();
            AllOrderItemsGrid.ItemsSource = orderItems;
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
    }
}