using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WpfAppRestoranOrder.Models;

namespace WpfAppRestoranOrder.Services
{
    public class DataService
    {
        private DatabaseService _dbService;
        public List<MenuItem> MenuItems { get; private set; }
        public List<Category> Categories { get; private set; }
        public List<Order> Orders { get; private set; }

        public DataService()
        {
            _dbService = new DatabaseService();
            MenuItems = new List<MenuItem>();
            Categories = new List<Category>();
            Orders = new List<Order>();
        }

        public void LoadAllData()
        {
            try
            {
               
                _dbService.InitializeDatabase();

               
                Categories = _dbService.GetCategories();

               
                MenuItems = _dbService.GetMenu();

               
                Orders = _dbService.GetOrders();
            }
            catch (Exception ex)
            {
              
            }
        }

        public void RefreshData()
        {
            LoadAllData();
        }

        public List<OrderItem> GetOrderItems()
        {
            return _dbService.GetOrderItems();
        }

        public bool AddCategory(Category category)
        {
            bool result = _dbService.AddCategory(category);
            if (result)
            {
                Categories = _dbService.GetCategories();
            }
            return result;
        }


        public bool UpdateCategory(Category category)
        {
            bool result = _dbService.UpdateCategory(category);
            if (result)
            {
                Categories = (_dbService.GetCategories());  
            }
            return result;  
        }

        public bool DeleteCategory(int categoryId)
        {
            bool result = _dbService.DeleteCategory(categoryId);
            if (result)
            {
                Categories = _dbService.GetCategories();
            }
            return result;
        }

        public bool AddMenuItem(MenuItem menuItem)
        {
           bool result = _dbService.AddMenuItem(menuItem);
            if (result)
            {
                MenuItems = _dbService.GetMenu();
            }
            return result;
        }

        public bool UpdateMenuItem(MenuItem menuItem)
        {
            bool result = _dbService.UpdateMenuItem(menuItem);
            if (result)
            {
                MenuItems = _dbService.GetMenu();
            }
            return result;
        }

        public bool DeleteMenuItem(int menuItemId)
        {
            bool result = _dbService.DeleteMenuItem(menuItemId);
            if (result)
            {
                MenuItems = _dbService.GetMenu();
            }
            return result;
        }
    }
}
