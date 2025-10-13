using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using WpfAppRestoranOrder.Models;

namespace WpfAppRestoranOrder.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            _connectionString = "Server=DESKTOP-QN9H26S\\SQLEXPRESS;Database=RestoranOrderDb;" +
                "User Id=sa;Password=1;TrustServerCertificate=true;";
        }

        public void InitializeDatabase()
        {
            try
            {
                string sqlScript = File.ReadAllText("SqlScripts/CreateTables.sql");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sqlScript, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                
            }
            catch (Exception ex)
            {
               
            }
        }

        public List<Category> GetCategories()
        {
            var categories = new List<Category>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Id, Name, ImageUrl FROM Categories", connection);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        ImageUrl = reader["ImageUrl"] as string ?? ""
                    });
                }
            }

            return categories;
        }

        public List<MenuItem> GetMenu()
        {
            var menuItems = new List<MenuItem>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand(@"
                    SELECT m.Id, m.Name, m.Price, m.Description, m.IsAvailable, m.ImageUrl, c.Name as Category 
                    FROM MenuItems m 
                    JOIN Categories c ON m.CategoryId = c.Id", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var menuItem = new MenuItem
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Price = (decimal)reader["Price"],
                            Description = reader["Description"] as string ?? "",
                            Category = (string)reader["Category"],
                            IsAvailable = (bool)reader["IsAvailable"],
                            ImageUrl = reader["ImageUrl"] as string ?? ""
                        };
                        menuItems.Add(menuItem);
                    }
                }
            }

            return menuItems;
        }

        public List<Order> GetOrders()
        {
            var orders = new List<Order>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(@"
                    SELECT o.Id, o.OrderDate, o.Status, o.TotalAmount, 
                    o.CustomerName, o.PhoneNumber, o.DeliveryAddress 
                    FROM Orders o", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        Id = (int)reader["Id"],
                        OrderDate = (DateTime)reader["OrderDate"],
                        Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), (string)reader["Status"]),
                        TotalAmount = (decimal)reader["TotalAmount"],
                        CustomerName = (string)reader["CustomerName"],
                        PhoneNumber = (string)reader["PhoneNumber"],
                        DeliveryAddress = reader["DeliveryAddress"] as string ?? ""
                    });
                }
            }
            return orders;
        }

        public List<OrderItem> GetOrderItems()
        {
            var orderItems = new List<OrderItem>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Id, OrderId, MenuItemId, Quantity, UnitPrice FROM OrderItems", connection);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orderItems.Add(new OrderItem
                    {
                        Id = (int)reader["Id"],
                        OrderId = (int)reader["OrderId"],
                        MenuItemId = (int)reader["MenuItemId"],
                        Quantity = (int)reader["Quantity"],
                        UnitPrice = (decimal)reader["UnitPrice"]
                    });
                }
            }

            return orderItems;
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}