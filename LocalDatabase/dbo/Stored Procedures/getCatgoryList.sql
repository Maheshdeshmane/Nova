 CREATE PROCEDURE getCatgoryList as
  select [CatgorieId]
      ,[CatgorieName]
  FROM [DisCheckOut].[dbo].[CheckOutCatgories]