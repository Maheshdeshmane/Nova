

 Create PROCEDURE [dbo].[getOnlineShopDetailsByName] 
 (@SEARCHTEXT VARCHAR(120))
AS
SELECT 
      [BusinessName]
      ,[BusinessContact]
      ,[latitude]
      ,[longitude]
      ,[shopAddress]
      ,[ShopName]
      ,[ShopType]
      ,[FacebookLink]
      ,[TwitterLink]
      ,[LinkdInLink]
      ,[HeaderImage]
      ,[HeaderImage2]
      ,[HeaderImage3]
      ,[ComplaintContact]
      ,[Email]
      ,[GoogleMapAddress]
      ,[WebBusinessName]
  FROM [DisCheckOut].[dbo].[CheckOutCustomer]
  where [WebBusinessName] =@SEARCHTEXT
  
  