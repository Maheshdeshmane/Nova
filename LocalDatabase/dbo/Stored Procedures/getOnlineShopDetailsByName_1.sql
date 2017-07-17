

 CREATE PROCEDURE [dbo].[getOnlineShopDetailsByName] 
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
      ,[WebBusinessName],ShopAboutUs,ShopValue1Tittle,ShopValue1Desc,ShopValue2Tittle,ShopValue2Desc,ShopValue3Tittle,ShopValue3Desc
	  ,ShopValue4Tittle,ShopValue4Desc,ShopOffersTagLine,ShopTeamTagLine,
	  ShopTestimonialsTagLine,ShopContactTagLine,ShopContactDesc,ShopTagSubLine
  FROM [DisCheckOut].[dbo].[CheckOutCustomer]
  where [WebBusinessName] =@SEARCHTEXT
  
  