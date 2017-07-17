CREATE TABLE [dbo].[CheckOutDiscount] (
    [customerId]        VARCHAR (10)  NOT NULL,
    [keyword]           VARCHAR (50)  NULL,
    [discount]          INT           NULL,
    [offer]             VARCHAR (120) NULL,
    [validFrom]         DATE          NULL,
    [validTill]         DATE          NULL,
    [images]            VARCHAR (120) NULL,
    [disId]             INT           NULL,
    [AdditionalDetails] VARCHAR (500) NULL,
    [SubCategoryId]     INT           NULL,
    FOREIGN KEY ([customerId]) REFERENCES [dbo].[CheckOutCustomer] ([CustomerId])
);

