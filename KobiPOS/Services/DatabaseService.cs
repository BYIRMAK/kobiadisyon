using Microsoft.Data.Sqlite;
using KobiPOS.Models;
using KobiPOS.Helpers;
using System.IO;

namespace KobiPOS.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;
        private static DatabaseService? _instance;

        public static DatabaseService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DatabaseService();
                }
                return _instance;
            }
        }

        private DatabaseService()
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "kobipos.db");
            var directory = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var createTables = @"
                CREATE TABLE IF NOT EXISTS Users (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Role TEXT NOT NULL,
                    FullName TEXT NOT NULL,
                    IsActive INTEGER NOT NULL DEFAULT 1
                );

                CREATE TABLE IF NOT EXISTS Tables (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    TableNumber INTEGER NOT NULL,
                    TableName TEXT NOT NULL,
                    Status TEXT NOT NULL DEFAULT 'Boş',
                    Capacity INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Categories (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    CategoryName TEXT NOT NULL,
                    DisplayOrder INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Products (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductName TEXT NOT NULL,
                    CategoryID INTEGER NOT NULL,
                    Price REAL NOT NULL,
                    StockQuantity INTEGER NOT NULL DEFAULT 0,
                    Barcode TEXT,
                    ImagePath TEXT,
                    FOREIGN KEY (CategoryID) REFERENCES Categories (ID)
                );

                CREATE TABLE IF NOT EXISTS Orders (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    TableID INTEGER NOT NULL,
                    OrderDate TEXT NOT NULL,
                    UserID INTEGER NOT NULL,
                    TotalAmount REAL NOT NULL,
                    DiscountAmount REAL NOT NULL DEFAULT 0,
                    PaymentType TEXT,
                    Status TEXT NOT NULL DEFAULT 'Bekliyor',
                    FOREIGN KEY (TableID) REFERENCES Tables (ID),
                    FOREIGN KEY (UserID) REFERENCES Users (ID)
                );

                CREATE TABLE IF NOT EXISTS OrderDetails (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    OrderID INTEGER NOT NULL,
                    ProductID INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    UnitPrice REAL NOT NULL,
                    Notes TEXT,
                    FOREIGN KEY (OrderID) REFERENCES Orders (ID),
                    FOREIGN KEY (ProductID) REFERENCES Products (ID)
                );

                CREATE TABLE IF NOT EXISTS Stock (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductID INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    TransactionType TEXT NOT NULL,
                    TransactionDate TEXT NOT NULL,
                    UserID INTEGER NOT NULL,
                    FOREIGN KEY (ProductID) REFERENCES Products (ID),
                    FOREIGN KEY (UserID) REFERENCES Users (ID)
                );

                CREATE TABLE IF NOT EXISTS Licenses (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    LicenseKey TEXT NOT NULL,
                    CustomerName TEXT NOT NULL,
                    HardwareID TEXT NOT NULL,
                    ActivationDate TEXT NOT NULL,
                    ExpiryDate TEXT NOT NULL,
                    IsActive INTEGER NOT NULL DEFAULT 1
                );

                CREATE TABLE IF NOT EXISTS AppSettings (
                    Key TEXT PRIMARY KEY,
                    Value TEXT NOT NULL
                );
            ";

            using var command = new SqliteCommand(createTables, connection);
            command.ExecuteNonQuery();

            SeedInitialData(connection);
        }

        private void SeedInitialData(SqliteConnection connection)
        {
            // Check if data already exists
            var checkCommand = new SqliteCommand("SELECT COUNT(*) FROM Users", connection);
            var userCount = Convert.ToInt32(checkCommand.ExecuteScalar());

            if (userCount > 0)
                return; // Data already seeded

            // Create default admin user
            var adminPassword = PasswordHelper.HashPassword("admin123");
            var insertAdmin = $@"
                INSERT INTO Users (Username, Password, Role, FullName, IsActive)
                VALUES ('admin', '{adminPassword}', 'Admin', 'Sistem Yöneticisi', 1);
            ";
            new SqliteCommand(insertAdmin, connection).ExecuteNonQuery();

            // Create sample users
            var garsonPassword = PasswordHelper.HashPassword("garson123");
            var kasiyerPassword = PasswordHelper.HashPassword("kasiyer123");
            var insertUsers = $@"
                INSERT INTO Users (Username, Password, Role, FullName, IsActive)
                VALUES 
                    ('garson1', '{garsonPassword}', 'Garson', 'Ahmet Yılmaz', 1),
                    ('kasiyer1', '{kasiyerPassword}', 'Kasiyer', 'Ayşe Demir', 1);
            ";
            new SqliteCommand(insertUsers, connection).ExecuteNonQuery();

            // Create tables (masa)
            var insertTables = @"
                INSERT INTO Tables (TableNumber, TableName, Status, Capacity)
                VALUES 
                    (1, 'Masa 1', 'Boş', 4),
                    (2, 'Masa 2', 'Boş', 4),
                    (3, 'Masa 3', 'Boş', 6),
                    (4, 'Masa 4', 'Boş', 2),
                    (5, 'Masa 5', 'Boş', 4),
                    (6, 'Masa 6', 'Boş', 8),
                    (7, 'Masa 7', 'Boş', 4),
                    (8, 'Masa 8', 'Boş', 6),
                    (9, 'Masa 9', 'Boş', 4),
                    (10, 'Masa 10', 'Boş', 2);
            ";
            new SqliteCommand(insertTables, connection).ExecuteNonQuery();

            // Create categories
            var insertCategories = @"
                INSERT INTO Categories (CategoryName, DisplayOrder)
                VALUES 
                    ('Kahveler', 1),
                    ('Soğuk İçecekler', 2),
                    ('Tatlılar', 3),
                    ('Ana Yemekler', 4),
                    ('Atıştırmalıklar', 5);
            ";
            new SqliteCommand(insertCategories, connection).ExecuteNonQuery();

            // Create products
            var insertProducts = @"
                INSERT INTO Products (ProductName, CategoryID, Price, StockQuantity, Barcode)
                VALUES 
                    ('Türk Kahvesi', 1, 25.00, 100, '1001'),
                    ('Espresso', 1, 30.00, 100, '1002'),
                    ('Cappuccino', 1, 35.00, 100, '1003'),
                    ('Latte', 1, 35.00, 100, '1004'),
                    ('Filtre Kahve', 1, 28.00, 100, '1005'),
                    ('Kola', 2, 20.00, 100, '2001'),
                    ('Ayran', 2, 15.00, 100, '2002'),
                    ('Portakal Suyu', 2, 25.00, 100, '2003'),
                    ('Limonata', 2, 22.00, 100, '2004'),
                    ('Su', 2, 10.00, 100, '2005'),
                    ('Sütlaç', 3, 40.00, 50, '3001'),
                    ('Kazandibi', 3, 45.00, 50, '3002'),
                    ('Künefe', 3, 60.00, 50, '3003'),
                    ('Baklava', 3, 55.00, 50, '3004'),
                    ('Hamburger', 4, 80.00, 50, '4001'),
                    ('Pizza', 4, 120.00, 50, '4002'),
                    ('Makarna', 4, 70.00, 50, '4003'),
                    ('Tavuk Şiş', 4, 90.00, 50, '4004'),
                    ('Patates Kızartması', 5, 35.00, 100, '5001'),
                    ('Soğan Halkası', 5, 40.00, 100, '5002');
            ";
            new SqliteCommand(insertProducts, connection).ExecuteNonQuery();

            // Set trial start date
            var trialStart = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var insertSettings = $@"
                INSERT INTO AppSettings (Key, Value)
                VALUES ('TrialStartDate', '{trialStart}');
            ";
            new SqliteCommand(insertSettings, connection).ExecuteNonQuery();
        }

        public SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        // User methods
        public User? ValidateUser(string username, string password)
        {
            using var connection = GetConnection();
            connection.Open();

            var command = new SqliteCommand(
                "SELECT * FROM Users WHERE Username = @username AND IsActive = 1",
                connection);
            command.Parameters.AddWithValue("@username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var user = new User
                {
                    ID = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    Role = reader.GetString(3),
                    FullName = reader.GetString(4),
                    IsActive = reader.GetInt32(5) == 1
                };

                if (PasswordHelper.VerifyPassword(password, user.Password))
                {
                    return user;
                }
            }

            return null;
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using var connection = GetConnection();
            connection.Open();

            var command = new SqliteCommand("SELECT * FROM Users", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new User
                {
                    ID = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    Role = reader.GetString(3),
                    FullName = reader.GetString(4),
                    IsActive = reader.GetInt32(5) == 1
                });
            }

            return users;
        }

        // Table methods
        public List<Table> GetAllTables()
        {
            var tables = new List<Table>();
            using var connection = GetConnection();
            connection.Open();

            var command = new SqliteCommand("SELECT * FROM Tables ORDER BY TableNumber", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                tables.Add(new Table
                {
                    ID = reader.GetInt32(0),
                    TableNumber = reader.GetInt32(1),
                    TableName = reader.GetString(2),
                    Status = reader.GetString(3),
                    Capacity = reader.GetInt32(4)
                });
            }

            return tables;
        }

        public void UpdateTableStatus(int tableId, string status)
        {
            using var connection = GetConnection();
            connection.Open();

            var command = new SqliteCommand(
                "UPDATE Tables SET Status = @status WHERE ID = @id",
                connection);
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@id", tableId);
            command.ExecuteNonQuery();
        }

        // Category methods
        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();
            using var connection = GetConnection();
            connection.Open();

            var command = new SqliteCommand("SELECT * FROM Categories ORDER BY DisplayOrder", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                categories.Add(new Category
                {
                    ID = reader.GetInt32(0),
                    CategoryName = reader.GetString(1),
                    DisplayOrder = reader.GetInt32(2)
                });
            }

            return categories;
        }

        // Product methods
        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            using var connection = GetConnection();
            connection.Open();

            var command = new SqliteCommand("SELECT * FROM Products", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product
                {
                    ID = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    CategoryID = reader.GetInt32(2),
                    Price = reader.GetDecimal(3),
                    StockQuantity = reader.GetInt32(4),
                    Barcode = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    ImagePath = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                });
            }

            return products;
        }

        public List<Product> GetProductsByCategory(int categoryId)
        {
            var products = new List<Product>();
            using var connection = GetConnection();
            connection.Open();

            var command = new SqliteCommand(
                "SELECT * FROM Products WHERE CategoryID = @categoryId",
                connection);
            command.Parameters.AddWithValue("@categoryId", categoryId);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product
                {
                    ID = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    CategoryID = reader.GetInt32(2),
                    Price = reader.GetDecimal(3),
                    StockQuantity = reader.GetInt32(4),
                    Barcode = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    ImagePath = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                });
            }

            return products;
        }

        // Order methods
        public int CreateOrder(Order order)
        {
            using var connection = GetConnection();
            connection.Open();

            var command = new SqliteCommand(@"
                INSERT INTO Orders (TableID, OrderDate, UserID, TotalAmount, DiscountAmount, PaymentType, Status)
                VALUES (@tableId, @orderDate, @userId, @totalAmount, @discountAmount, @paymentType, @status);
                SELECT last_insert_rowid();
            ", connection);

            command.Parameters.AddWithValue("@tableId", order.TableID);
            command.Parameters.AddWithValue("@orderDate", order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@userId", order.UserID);
            command.Parameters.AddWithValue("@totalAmount", order.TotalAmount);
            command.Parameters.AddWithValue("@discountAmount", order.DiscountAmount);
            command.Parameters.AddWithValue("@paymentType", order.PaymentType);
            command.Parameters.AddWithValue("@status", order.Status);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void AddOrderDetail(OrderDetail detail)
        {
            using var connection = GetConnection();
            connection.Open();

            var command = new SqliteCommand(@"
                INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice, Notes)
                VALUES (@orderId, @productId, @quantity, @unitPrice, @notes)
            ", connection);

            command.Parameters.AddWithValue("@orderId", detail.OrderID);
            command.Parameters.AddWithValue("@productId", detail.ProductID);
            command.Parameters.AddWithValue("@quantity", detail.Quantity);
            command.Parameters.AddWithValue("@unitPrice", detail.UnitPrice);
            command.Parameters.AddWithValue("@notes", detail.Notes);

            command.ExecuteNonQuery();
        }

        // Settings methods
        public string? GetSetting(string key)
        {
            using var connection = GetConnection();
            connection.Open();

            var command = new SqliteCommand(
                "SELECT Value FROM AppSettings WHERE Key = @key",
                connection);
            command.Parameters.AddWithValue("@key", key);

            var result = command.ExecuteScalar();
            return result?.ToString();
        }

        public void SetSetting(string key, string value)
        {
            using var connection = GetConnection();
            connection.Open();

            var command = new SqliteCommand(@"
                INSERT OR REPLACE INTO AppSettings (Key, Value)
                VALUES (@key, @value)
            ", connection);

            command.Parameters.AddWithValue("@key", key);
            command.Parameters.AddWithValue("@value", value);
            command.ExecuteNonQuery();
        }
    }
}
