Create DataBase ShopRefrigerators_bd
use ShopRefrigerators_bd

Create TABLE dbo.[ORDER](
	ID int not null Identity Primary Key,
	Buyer nvarchar(50) not null,
	Seller nvarchar(50) not null,
	[Date] date not null default getdate(),
	ID_Refrigerators int not null Foreign Key References Refrigerators(Id)
)

Create Table dbo.Refrigerators(
	Id int not null Identity Primary Key,
	Manufacturer nvarchar(50) not null,
	Model nvarchar(50) not null,
	Price int not null,
	Quantity int not null,
	CHECK(Price>0 AND Quantity>=0))



