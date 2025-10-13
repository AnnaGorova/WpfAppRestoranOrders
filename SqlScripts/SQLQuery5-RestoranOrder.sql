USE RestoranOrderDb;
GO 

CREATE TABLE categories (
    Id INT NOT NULL IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    ImageUrl NVARCHAR(500) NULL
);
GO 

INSERT INTO categories  VALUES ('Піца');
GO 
INSERT INTO categories  VALUES ('Бургери');
GO 
INSERT INTO categories  VALUES ('Салати');
GO 
INSERT INTO categories  VALUES ('Десерти');
GO 

CREATE TABLE menuItems (
    Id INT NOT NULL IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Price DECIMAL(10,2) NOT NULL CHECK (Price > 0),
    CategoryId INT NOT NULL,
    IsAvailable BIT DEFAULT 1,
    ImageUrl NVARCHAR(500),
    FOREIGN KEY (CategoryId) REFERENCES categories(Id)
);
GO 

INSERT INTO menuItems (Name, Description, Price, CategoryId, ImageUrl) VALUES
('Тірамісу', 'Класичний італійський десерт з маскарпоне та кавою', 89.00, 4, 'https://lasunka.com/s165-prew.jpg');
GO 
INSERT INTO menuItems (Name, Description, Price, CategoryId, ImageUrl) VALUES
('Чізкейк Нью-Йорк', 'Ніжний чізкейк з ягідним соусом', 79.00, 4, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRnoJJRbsaYLx_EQHAuMyasWc5pPi69z0K1ow&s')
GO 
INSERT INTO menuItems (Name, Description, Price, CategoryId, ImageUrl) VALUES
('Шоколадний фондан', 'Теплий шоколадний кекс з рідкою начинкою', 69.00, 4, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQorpJnieWO0imrpB8qOX88Y6-8xl6dLbf-Uw&s')
GO 

INSERT INTO menuItems (Name, Description, Price, CategoryId, ImageUrl) VALUES
('Морозиво', '3 кульки морозива на вибір: ваніль, шоколад, полуниця', 59.00, 4, 'https://i.evrasia.in.ua/data/1400_0/products/nytznciVTAtFnjq1tmKAQmhWXka2TGcX3kwXRJCb.webp')
GO 


INSERT INTO menuItems (Name, Description, Price, CategoryId, ImageUrl) VALUES
('Цезар з куркою', 'Салат ромен, куряче філе, крутони, пармезан, соус Цезар', 149.00, 3, 'https://klopotenko.com/wp-content/uploads/2022/01/cezar-z-kyrkou-i-tomatamu_sitewebukr-1000x600.jpg?v=1720546600')
GO 
INSERT INTO menuItems (Name, Description, Price, CategoryId, ImageUrl) VALUES
('Грецький салат', 'Помідори, огірки, оливки, сир фета, оливкова олія', 129.00, 3, 'https://horodok.city/upload/article/kAhOV8LQStsPeOhgTUxr.jpg')
GO 
INSERT INTO menuItems (Name, Description, Price, CategoryId, ImageUrl) VALUES
('Класичний бургер', 'Яловича котлета, сир чеддер, салат, помідор, цибуля', 129.00, 2, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRyLwRCUSl9Y3dVDoJYl1aprxiBScB1V5ZdBg&s')
GO 
INSERT INTO menuItems (Name, Description, Price, CategoryId, ImageUrl) VALUES
('Чізбургер', 'Подвійна яловича котлета, сир чеддер, бекон, соус BBQ', 169.00, 2, 'https://sfood.in.ua/wp-content/uploads/2020/10/B-3-e1630391342951.jpg')
GO 

INSERT INTO menuItems (Name, Description, Price, CategoryId, ImageUrl) VALUES
('Маргарита', 'Класична італійська піца з томатним соусом, моцарелою та свіжим базиліком', 199.00, 1, 'https://klopotenko.com/wp-content/uploads/2023/03/pitsa-marharyta_sitewebukr-img-1000x600.jpg?v=1720545473')
GO 
INSERT INTO menuItems (Name, Description, Price, CategoryId, ImageUrl) VALUES
('Пепероні', 'Піца з салямі пепероні, моцарелою та томатним соусом', 229.00, 1, 'https://roll-club.kh.ua/wp-content/uploads/2022/12/5.jpg')
GO 


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
GO 

INSERT INTO orders (OrderDate, Status, CustomerName, PhoneNumber, TotalAmount, DeliveryAddress) VALUES
('2025-10-07 16:20:00', 'New', 'Петро Іваненко', '+380441112233', 150.00, 'вул. Лісова, 5')
GO 
INSERT INTO orders (OrderDate, Status, CustomerName, PhoneNumber, TotalAmount, DeliveryAddress) VALUES
('2025-10-07 13:10:00', 'Cancelled', 'Софія Мельник', '+380639998877', 89.00, NULL);
GO 
INSERT INTO orders (OrderDate, Status, TotalAmount, CustomerName, PhoneNumber, DeliveryAddress) VALUES 
('2025-10-07 14:30:00', 'InProgress', 447.00, 'Іван Петренко', '+380991234567', 'вул. Хрещатик, 25, кв. 42');
GO 
INSERT INTO orders (OrderDate, Status, TotalAmount, CustomerName, PhoneNumber, DeliveryAddress) VALUES 
('2025-10-06 19:15:00', 'Completed', 328.00, 'Марія Коваленко', '+380667894561', NULL);
GO 
INSERT INTO orders (CustomerName, PhoneNumber, DeliveryAddress, TotalAmount) VALUES 
('Олександр Шевченко', '+380931112233', 'просп. Перемоги, 10, кв. 17', 189.00);
GO 
INSERT INTO orders (OrderDate, Status, CustomerName, PhoneNumber, TotalAmount) VALUES 
('2025-10-07 15:45:00', 'Ready', 'Наталія Бойко', '+380509876543', 275.50);
GO 

CREATE TABLE orderItems (
    Id INT NOT NULL IDENTITY PRIMARY KEY,
    OrderId INT NOT NULL,
    MenuItemId INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(10,2) NOT NULL CHECK (UnitPrice >= 0),
    FOREIGN KEY (OrderId) REFERENCES orders(Id),
    FOREIGN KEY (MenuItemId) REFERENCES menuItems(Id)
);
GO 
INSERT INTO orderItems (OrderId, MenuItemId, Quantity, UnitPrice) VALUES
(1, 1, 1, 199.00);  -- Маргарита
GO 
INSERT INTO orderItems (OrderId, MenuItemId, Quantity, UnitPrice) VALUES
(1, 6, 2, 129.00); -- Класичний бургер
GO 
INSERT INTO orderItems (OrderId, MenuItemId, Quantity, UnitPrice) VALUES
(2, 3, 2, 129.00); -- Тірамісу
GO 
   









--ALTER TABLE menuItems 
--ADD ImageUrl NVARCHAR(500) NULL;  -- вже внесено в таблицю 

--ALTER TABLE categories 
--ADD ImageUrl NVARCHAR(500) NULL; -- вже внесено в таблицю 



SELECT * FROM categories;
SELECT * FROM menuItems;
SELECT * FROM orders;
SELECT * FROM orderItems;
