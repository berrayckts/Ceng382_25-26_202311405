IF OBJECT_ID(N'dbo.ShippersContactInfos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ShippersContactInfos
    (
        ShippersContactInfoId INT IDENTITY(1,1) NOT NULL,
        Email NVARCHAR(150) NOT NULL,
        Website NVARCHAR(200) NOT NULL,
        Phone NVARCHAR(24) NULL,
        Address NVARCHAR(120) NULL,
        City NVARCHAR(50) NULL,
        Country NVARCHAR(50) NULL,
        PostalCode NVARCHAR(20) NULL,
        Shipper INT NOT NULL,
        CustomerId NCHAR(5) NULL,
        CONSTRAINT PK_ShippersContactInfos PRIMARY KEY (ShippersContactInfoId),
        CONSTRAINT FK_ShippersContactInfos_Shippers
            FOREIGN KEY (Shipper) REFERENCES dbo.Shippers(ShipperID)
    );
END;
