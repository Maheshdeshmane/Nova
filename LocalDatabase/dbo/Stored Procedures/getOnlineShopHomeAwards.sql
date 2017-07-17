


 Create PROCEDURE [dbo].[getOnlineShopHomeAwards] 
 (@SEARCHTEXT VARCHAR(120))
AS
SELECT 
      *
  FROM [DisCheckOut].[dbo].[CheckOutAwards] A
  INNER JOIN [DisCheckOut].[dbo].[CheckOutCustomer] C
  ON A.[CustomerId]=c.[CustomerId]
  where C.[WebBusinessName] =@SEARCHTEXT
  
  
