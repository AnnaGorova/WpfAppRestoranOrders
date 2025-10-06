USE RestoranOrderDb;

CREATE TABLE categories (
    Id INT NOT NULL IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE menuItems (
    Id INT NOT NULL IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Price DECIMAL(10,2) NOT NULL CHECK (Price > 0),
    CategoryId INT NOT NULL,
    IsAvailable BIT DEFAULT 1,
    FOREIGN KEY (CategoryId) REFERENCES categories(Id)
);

CREATE TABLE orders (
    Id INT NOT NULL IDENTITY PRIMARY KEY,
    OrderDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(20) NOT NULL DEFAULT 'New' 
        CHECK (Status IN ('New','InProgress','Ready','Completed','Cancelled')),
    TotalAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
    CustomerName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL,
    DeliveryAddress NVARCHAR(255) NULL
);

CREATE TABLE orderItems (
    Id INT NOT NULL IDENTITY PRIMARY KEY,
    OrderId INT NOT NULL,
    MenuItemId INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(10,2) NOT NULL CHECK (UnitPrice >= 0),
    FOREIGN KEY (OrderId) REFERENCES orders(Id),
    FOREIGN KEY (MenuItemId) REFERENCES menuItems(Id)
);

INSERT INTO categories  VALUES ('Піца');
INSERT INTO categories  VALUES ('Бургери');
INSERT INTO categories  VALUES ('Салати');
INSERT INTO categories  VALUES ('Десерти');

SELECT * FROM categories;
SELECT * FROM menuItems;
SELECT * FROM orders;
SELECT * FROM orderItems;
