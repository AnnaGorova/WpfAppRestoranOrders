using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using WpfAppRestoranOrder.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Linq.Expressions;

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
                    using (var command = new SqlCommand(sqlScript, connection))  // ✅ Використовуємо SqlCommand
                    {
                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("Базу даних успішно створено!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        public List<Models.Category> GetCategories()
        {
            var categories = new List<Models.Category>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Id, Name FROM Categories", connection);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    categories.Add(new Models.Category
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
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
                        SELECT m.Id, m.Name, m.Price, m.Description, m.IsAvailable, c.Name as Category 
                        FROM MenuItems m 
                        JOIN Categories c ON m.CategoryId = c.Id", connection);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var menuItem = new MenuItem  
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Price = (decimal)reader["Price"],
                        Description = reader["Description"] as string ?? "",
                        Category = (string)reader["Category"],
                        IsAvailable = (bool)reader["IsAvailable"]
                    };

                    menuItems.Add(menuItem);  
                }
            }

            return menuItems;
        }


        public List<Models.Order> GetOrders()
        {
            var orders = new List<Models.Order>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Id, OrderDate, Status, TotalAmount, CustomName, PhoneNumber, DeliveryAddress FROM Orders", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Models.Order
                    {
                        Id = (int)reader["Id"],
                        OrderDate = (DateTime)reader["OrderDate"],
                        Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), (string)reader["Status"]),
                        TotalAmount = (decimal)reader["TotalAmount"],
                        CustomName = (string)reader["CustomName"],  // ✅ Тепер правильно
                        PhoneNumber = (string)reader["PhoneNumber"],
                        DeliveryAddress = reader["DeliveryAddress"] as string ?? ""
                    });
                }
            }
            return orders;
        }



        public List<Models.OrderItem> GetOrderItems()  
        {
            var orderItems = new List<Models.OrderItem>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Id, OrderId, MenuItemId, Quantity, UnitPrice FROM OrderItems", connection);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orderItems.Add(new Models.OrderItem  
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

    }
}