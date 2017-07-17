 create PROCEDURE [dbo].[getOnlineShopDiscountByName] (@SEARCHTEXT VARCHAR(120))
AS
SELECT 
           [keyword]
      ,[discount]
      ,[offer]
      ,[validFrom]
      ,[validTill]
      ,[AdditionalDetails]
  FROM [DisCheckOut].[dbo].[CheckOutDiscount]D
  inner join dbo.CheckOutCustomer C on D.CustomerId=C.CustomerId 
  where C.WebBusinessName =@SEARCHTEXT
  