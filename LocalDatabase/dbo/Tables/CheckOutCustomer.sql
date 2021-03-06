﻿CREATE TABLE [dbo].[CheckOutCustomer] (
    [CustomerId]       VARCHAR (10)    NOT NULL,
    [BusinessName]     VARCHAR (50)    NULL,
    [BusinessContact]  VARCHAR (20)    NULL,
    [latitude]         DECIMAL (12, 9) NULL,
    [longitude]        DECIMAL (12, 9) NULL,
    [shopAddress]      VARCHAR (120)   NULL,
    [ShopName]         VARCHAR (50)    NULL,
    [ShopType]         INT             NULL,
    [FacebookLink]     VARCHAR (100)   NULL,
    [TwitterLink]      VARCHAR (100)   NULL,
    [LinkdInLink]      VARCHAR (100)   NULL,
    [HeaderImage]      VARCHAR (50)    NULL,
    [HeaderImage2]     VARCHAR (50)    NULL,
    [HeaderImage3]     VARCHAR (50)    NULL,
    [ComplaintContact] VARCHAR (10)    NULL,
    [Email]            VARCHAR (100)   NULL,
    [GoogleMapAddress] VARCHAR (500)   NULL,
    [WebBusinessName]  VARCHAR (100)   NULL,
    PRIMARY KEY CLUSTERED ([CustomerId] ASC),
    CONSTRAINT [CK_MyTable_PhoneNumber] CHECK ([CustomerId] like '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
);

