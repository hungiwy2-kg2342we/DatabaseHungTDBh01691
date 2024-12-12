CREATE DATABASE ASM_DataBase_SE07203;
GO	

USE ASM_DataBase_SE07203;
GO	
CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL
);
CREATE TABLE Product (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductCode NVARCHAR(50) NOT NULL,
    ProductName NVARCHAR(100) NOT NULL,
    Price  DECIMAL(15, 2) NOT NULL,
    InventoryQuantity INT NOT NULL,
    ProductImage NVARCHAR(MAX),
    CategoryID INT,
    FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);
CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    EmployeeCode NVARCHAR(50) NOT NULL,
    EmployeeName NVARCHAR(100) NOT NULL,
    Position NVARCHAR(50),
    AuthorityLevel NVARCHAR(50) NOT NULL,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    PasswordChanged BIT DEFAULT 0
);
INSERT INTO [dbo].[Employee] 
VALUES ('Emp1', 'Nguyen Van A', 'Manager', 'Admin', 'admin1','123456', 1);

INSERT INTO [dbo].[Employee] 
VALUES ('Emp2', 'Nguyen Van B', 'Staff', 'Warehouse Manager', 'warehousemanager1','123456', 1),
		('Emp3', 'Nguyen Van C', 'Staff', 'Sale', 'sale1','123456', 0);

SELECT * From [dbo].[Employee];

CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    CustomerCode NVARCHAR(50) NOT NULL,
    CustomerName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20),
    Address NVARCHAR(255)
);
INSERT INTO Customer (CustomerCode, CustomerName, PhoneNumber, Address) 
VALUES
('C001', 'Tran Duc Hung', '012343123', 'Hai Phong' ),
('C002', 'Tran Thi Mai', '0987654321', 'Ho Chi Minh City' ),
('C003', 'Luong The Hung', '0123456789', 'Lao Cai' ),
('C004', 'Tran Thi Kim Linh', '0987654321', 'Ho Chi Minh');

SELECT * FROM Customer

CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    OrderDate DATETIME NOT NULL,
    EmployeeID INT,
    CustomerID INT,
    FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);	
INSERT INTO Orders (OrderDate, EmployeeID, CustomerID) VALUES
('2024-01-01 10:00:00', 1, 1),  -- Replace with actual EmployeeID and CustomerID
('2024-01-02 11:30:00', 2, 2),  -- Replace with actual EmployeeID and CustomerID
('2024-01-03 14:45:00', 3, 1),  -- Replace with actual EmployeeID and CustomerID
('2024-01-04 09:15:00', 1, 3);  -- Replace with actual EmployeeID and CustomerID

Select * From Orders
CREATE TABLE OrderDetail (
    OrderDetailID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT,
    ProductID INT,
    QuantitySold INT NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
);

INSERT INTO OrderDetail (OrderID, ProductID, QuantitySold) VALUES
(1, 1, 7),
(1, 2, 9),
(2, 3,4);
SELECT * FROM [dbo]. [OrderDetail]

UPDATE Product
SET price = 150000000
WHERE productCode = 'P002';

CREATE TABLE Import (
    ImportID INT PRIMARY KEY IDENTITY(1,1),
    ImportDate DATETIME NOT NULL,
    EmployeeID INT,
    FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID)
);
INSERT INTO Import (EmployeeID,ImportDate) 
VALUES
(1,'2024-10-2'),
(2,'2024-10-7');
SELECT * FROM [dbo]. [Import];

CREATE TABLE ImportDetail (
    ImportDetailID INT PRIMARY KEY IDENTITY(1,1),
    ImportID INT,
    ProductID INT,
    QuantityImported INT NOT NULL,
    ImportCost DECIMAL(10, 2),
    FOREIGN KEY (ImportID) REFERENCES Import(ImportID),
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
);
INSERT INTO ImportDetail (ImportID, ProductID, QuantityImported, ImportCost) VALUES
(01, 01, 123, 100000000),
(02, 02, 90, 120000000),
(03, 03, 90, 410000000),
(04, 04, 42, 900000000);
SELECT * FROM [dbo]. [ImportDetail];


INSERT INTO Category (CategoryName)
VALUES 
('Sport'),
('Pickup truck'),
('Family car'),
('Truck'),
('Camp');
select * from Category

Delete From Category
Where CategoryID = 5;

INSERT INTO Product (ProductCode, ProductName, Price, InventoryQuantity, ProductImage, CategoryID)
VALUES 
('P001', 'Mescerdes', 2000000000, 100, NULL, 1),  -- CategoryID 1: Sport
('P002', 'MG', 150000000, 57, NULL, 1), -- CategoryID 1: Sport
('P003', 'Ford', 450000000, 123, NULL, 2), -- CategoryID 2: Pickup truck
('P004', 'Mazda', 950000000, 63, NULL, 3),    -- CategoryID 3: Family car
('P005', 'Suzuki', 450000000, 24, NULL, 4);   -- CategoryID 4: Truck

select * from Product

ALTER TABLE Product
ALTER COLUMN Price DECIMAL(15, 2);

DECLARE @employeeID INT = 1; 

SELECT 
    o.OrderDate, 
    e.EmployeeName, 
    c.CustomerName
FROM 
    Orders o
INNER JOIN 
    Employee e ON o.EmployeeID = e.EmployeeID
INNER JOIN 
    Customer c ON o.CustomerID = c.CustomerID
WHERE 
    o.EmployeeID = 1; 




