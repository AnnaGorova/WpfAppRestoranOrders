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
            MessageBox.Show("Додати страву - функція в розробці", "Інформація");
        }

        private void EditMenuItemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AllMenuGrid.SelectedItem is Models.MenuItem selectedItem)
            {
                MessageBox.Show($"Редагувати страву: {selectedItem.Name}", "Інформація");
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
                var result = MessageBox.Show($"Видалити страву '{selectedItem.Name}'?",
                    "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show($"Страва '{selectedItem.Name}' видалена", "Інформація");
                }
            }
            else
            {
                MessageBox.Show("Виберіть страву для видалення", "Попередження");
            }
        }

        private void RefreshMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            _dataService.RefreshData();
            AllMenuGrid.ItemsSource = _dataService.MenuItems;
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