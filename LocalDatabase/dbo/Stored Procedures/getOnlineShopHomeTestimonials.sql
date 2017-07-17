


 Create PROCEDURE [dbo].[getOnlineShopHomeTestimonials] 
 (@SEARCHTEXT VARCHAR(120))
AS
SELECT 
      *
  FROM [dbo].[CheckOutTestimonials] T
  INNER JOIN [DisCheckOut].[dbo].[CheckOutCustomer] C
  ON T.[CustomerId]=c.[CustomerId]
  where C.[WebBusinessName] =@SEARCHTEXT
  
  
