CREATE TABLE [dbo].[CheckOutSubCatgories] (
    [SubCatgorieId]   INT          NOT NULL,
    [SubCatgorieName] VARCHAR (20) NOT NULL,
    [CatgorieId]      INT          NOT NULL,
    PRIMARY KEY CLUSTERED ([SubCatgorieId] ASC),
    FOREIGN KEY ([CatgorieId]) REFERENCES [dbo].[CheckOutCatgories] ([CatgorieId])
);

