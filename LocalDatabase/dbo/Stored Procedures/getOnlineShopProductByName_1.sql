 CREATE PROCEDURE [dbo].[getOnlineShopProductByName] (@SEARCHTEXT VARCHAR(120))
AS

SELECT [ProductId]
      ,[FolderName]
      ,[ProductImageName]
      ,[ProductDesc]
      ,[ProductPrice]
      ,[ProductName]
      ,[ProductStar]
  FROM [DisCheckOut].[dbo].[CheckOutShopProducts] P
  inner join dbo.CheckOutCustomer C on P.CustomerId=C.CustomerId 
  where C.WebBusinessName =@SEARCHTEXT ORDER BY P.FolderName,P.ProductStar DESC