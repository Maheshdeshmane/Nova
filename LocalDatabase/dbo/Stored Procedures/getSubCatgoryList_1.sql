/****** Script for SelectTopNRows command from SSMS  ******/

  
  CREATE procedure [dbo].[getSubCatgoryList]  
  as
  SELECT [SubCatgorieId]
      ,[SubCatgorieName]
      ,[CatgorieId]
  FROM [DisCheckOut].[dbo].[CheckOutSubCatgories]
  