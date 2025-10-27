using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Documents;
using WpfAppRestoranOrder.Models;
using WpfAppRestoranOrder.View.Client;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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



        public bool AddCategory(Category category)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "INSERT INTO categories (Name, ImageUrl) VALUES (@Name, @ImageUrl)", connection);

                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.Parameters.AddWithValue("@ImageUrl", category.ImageUrl ?? "");

                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool UpdateCategory(Category category)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE categories SET Name = @Name, ImageUrl = @ImageUrl WHERE Id = @Id", connection);

                    command.Parameters.AddWithValue("@Id", category.Id);
                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.Parameters.AddWithValue("@ImageUrl", category.ImageUrl);

                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteCategory(int categoryId) 
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("DELETE FROM Categories WHERE Id = @Id",  connection); 
                    command.Parameters.AddWithValue("@Id", categoryId);

                    int result = command.ExecuteNonQuery();
                    return result > 0;  
                }
            }
            catch (Exception ex) 
            {
                return false;
            }
        }


        public bool AddMenuItem(MenuItem menuItem)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "INSERT INTO MenuItems (Name, Description, Price, " +
                        "CategoryId, IsAvailable, ImageUrl) " +
                        "VALUES (@Name, @Description, @Price, " +
                        "(SELECT Id FROM Categories WHERE Name = @Category), @IsAvailable, @ImageUrl)",
                        connection);

                    command.Parameters.AddWithValue("@Name", menuItem.Name);
                    command.Parameters.AddWithValue("@Description", menuItem.Description ?? "");
                    command.Parameters.AddWithValue("@Price", menuItem.Price);
                    command.Parameters.AddWithValue("@Category", menuItem.Category);
                    command.Parameters.AddWithValue("@IsAvailable", menuItem.IsAvailable);
                    command.Parameters.AddWithValue("@ImageUrl", menuItem.ImageUrl ?? "");

                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateMenuItem(MenuItem menuItem)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE MenuItems SET Name = @Name, Description = @Description, " +
                        "Price = @Price, CategoryId = " +
                        "(SELECT Id FROM Categories WHERE Name = @Category), IsAvailable = @IsAvailable, ImageUrl = @ImageUrl WHERE Id = @Id",
                        connection);

                    command.Parameters.AddWithValue("@Id", menuItem.Id);
                    command.Parameters.AddWithValue("@Name", menuItem.Name);
                    command.Parameters.AddWithValue("@Description", menuItem.Description ?? "");
                    command.Parameters.AddWithValue("@Price", menuItem.Price);
                    command.Parameters.AddWithValue("@Category", menuItem.Category);
                    command.Parameters.AddWithValue("@IsAvailable", menuItem.IsAvailable);
                    command.Parameters.AddWithValue("@ImageUrl", menuItem.ImageUrl ?? "");

                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteMenuItem(int menuItemId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("DELETE FROM MenuItems WHERE Id = @Id", connection);
                    command.Parameters.AddWithValue("@Id", menuItemId);

                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool UpdateOrderContacts(Order order)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "UPDATE Orders SET CustomerName = @CustomerName, " +
                        "PhoneNumber = @PhoneNumber, " +
                        "DeliveryAddress = @DeliveryAddress WHERE Id = @Id", connection);
                    
                    command.Parameters.AddWithValue("@Id", order.Id);
                    
                    command.Parameters.AddWithValue("@CustomerName", order.CustomerName);
                    command.Parameters.AddWithValue("@PhoneNumber", order.PhoneNumber);
                    command.Parameters.AddWithValue("@DeliveryAddress", order.DeliveryAddress ?? "");



                    int result = command.ExecuteNonQuery();
                    return result > 0;

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool UpdateOrderStatus(Order order)
        {
            try
            {
                using (var connecting = new SqlConnection(_connectionString))
                {
                    connecting.Open();
                    var command = new SqlCommand(
                        "UPDATE Orders SET Status = @Status WHERE Id = @Id", connecting);
                    command.Parameters.AddWithValue("@Id", order.Id);
                    command.Parameters.AddWithValue("@Status", order.Status.ToString());

                    int result = command.ExecuteNonQuery();
                    return result > 0;

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DleteOrder(int orderId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    
                    var deleteOrderItemsCommand = new SqlCommand(
                        "DELETE FROM OrderItems WHERE OrderId = @OrderId", connection);
                    deleteOrderItemsCommand.Parameters.AddWithValue("@OrderId", orderId);
                    deleteOrderItemsCommand.ExecuteNonQuery();

                    
                    var deleteOrderCommand = new SqlCommand(
                        "DELETE FROM Orders WHERE Id = @Id", connection);
                    deleteOrderCommand.Parameters.AddWithValue("@Id", orderId);

                    int result = deleteOrderCommand.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }













        public bool CreateOrder(Order order, List<CartItem> cartItems)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    
                    var maxIdCommand = new SqlCommand("SELECT MAX(Id) FROM orders", connection);
                    var maxIdResult = maxIdCommand.ExecuteScalar();
                    var maxOrderId = maxIdResult == DBNull.Value ? 0 : Convert.ToInt32(maxIdResult);

                    
                    var currentIdentityCommand = new SqlCommand("SELECT IDENT_CURRENT('orders')", connection);
                    var currentIdentity = Convert.ToInt32(currentIdentityCommand.ExecuteScalar());

                    if (currentIdentity >= 1000 || currentIdentity <= maxOrderId)
                    {
                        var newSeed = maxOrderId + 1;
                        var resetCommand = new SqlCommand($"DBCC CHECKIDENT ('orders', RESEED, {newSeed})", connection);
                        resetCommand.ExecuteNonQuery();
                    }

                    
                    var insertOrderCommand = new SqlCommand(
                        @"INSERT INTO Orders (OrderDate, Status, TotalAmount, CustomerName, PhoneNumber, DeliveryAddress) 
                          VALUES (@OrderDate, @Status, @TotalAmount, @CustomerName, @PhoneNumber, @DeliveryAddress);
                          SELECT SCOPE_IDENTITY();", connection);
                   

                    insertOrderCommand.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                    insertOrderCommand.Parameters.AddWithValue("@Status", order.Status.ToString());
                    insertOrderCommand.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                    insertOrderCommand.Parameters.AddWithValue("@CustomerName", order.CustomerName);
                    insertOrderCommand.Parameters.AddWithValue("@PhoneNumber", order.PhoneNumber);
                    insertOrderCommand.Parameters.AddWithValue("@DeliveryAddress", order.DeliveryAddress ?? (object)DBNull.Value);

                    var orderId = Convert.ToInt32(insertOrderCommand.ExecuteScalar());

                    
                    foreach (var cartItem in cartItems)
                    {
                        var insertOrderItemCommand = new SqlCommand(
                            @"INSERT INTO OrderItems (OrderId, MenuItemId, Quantity, UnitPrice) 
                              VALUES (@OrderId, @MenuItemId, @Quantity, @UnitPrice)", connection);

                        insertOrderItemCommand.Parameters.AddWithValue("@OrderId", orderId);
                        insertOrderItemCommand.Parameters.AddWithValue("@MenuItemId", cartItem.MenuItemId);
                        insertOrderItemCommand.Parameters.AddWithValue("@Quantity", cartItem.Quantity);
                        insertOrderItemCommand.Parameters.AddWithValue("@UnitPrice", cartItem.Price);

                        insertOrderItemCommand.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
               
                return false;
            }
        }
















        

    }
}