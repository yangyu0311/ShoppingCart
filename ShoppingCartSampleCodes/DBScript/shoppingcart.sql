Create Table Product 
(
	ID int IDENTITY(1,1) PRIMARY KEY,
	Name varchar(100) NOT NULL,
	Baseprice decimal(18, 2)   
);

Create Table Cart (
	ID int IDENTITY(1,1) PRIMARY KEY,
	CreateTime DateTime
);

Create Table SaleItem (
	ID  int IDENTITY(1,1) PRIMARY KEY,
	ProductID int NOT NULL, 
	CartID int NOT NULL,
	Quantity int,
	FOREIGN KEY (CartID) REFERENCES Cart(ID)
);




