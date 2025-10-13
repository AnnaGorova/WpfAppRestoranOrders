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
    }
}
