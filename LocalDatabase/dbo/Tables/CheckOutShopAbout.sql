CREATE TABLE [dbo].[CheckOutShopAbout] (
    [CustomerId]        VARCHAR (10)  NOT NULL,
    [PersonName]        VARCHAR (50)  NOT NULL,
    [PersonDesignation] VARCHAR (30)  NOT NULL,
    [PersonImage]       VARCHAR (50)  NOT NULL,
    [PersonRole]        VARCHAR (500) NOT NULL,
    [PersonInLevel]     INT           NOT NULL,
    [FacebookLink]      VARCHAR (100) NULL,
    [TwitterLink]       VARCHAR (100) NULL,
    [LinkdInLink]       VARCHAR (100) NULL,
    FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[CheckOutCustomer] ([CustomerId])
);

