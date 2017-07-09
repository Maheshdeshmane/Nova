 CREATE PROCEDURE [dbo].[getOnlineShopTeamByName] (@SEARCHTEXT VARCHAR(120))
AS
SELECT 
      A.[PersonName]
      ,A.[PersonDesignation]
      ,A.[PersonImage]
      ,A.[PersonRole]
      ,A.[PersonInLevel]
      ,A.[FacebookLink]
      ,A.[TwitterLink]
      ,A.[LinkdInLink]
  FROM [DisCheckOut].[dbo].[CheckOutShopAbout] A
  inner join dbo.CheckOutCustomer C on A.CustomerId=C.CustomerId 
  where C.WebBusinessName =@SEARCHTEXT
  ORDER BY A.PersonInLevel