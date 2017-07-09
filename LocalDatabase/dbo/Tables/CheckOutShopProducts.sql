CREATE TABLE [dbo].[CheckOutShopProducts] (
    [ProductId]        INT           IDENTITY (1, 1) NOT NULL,
    [CustomerId]       VARCHAR (10)  NOT NULL,
    [FolderName]       VARCHAR (20)  NOT NULL,
    [ProductImageName] VARCHAR (20)  NOT NULL,
    [ProductDesc]      VARCHAR (200) NOT NULL,
    [ProductPrice]     INT           NOT NULL,
    [ProductName]      VARCHAR (100) NULL,
    [ProductStar]      INT           NULL,
    PRIMARY KEY CLUSTERED ([ProductId] ASC),
    FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[CheckOutCustomer] ([CustomerId])
);

