CREATE TABLE [dbo].[CheckOutTransation] (
    [customerId]  VARCHAR (10) NOT NULL,
    [payment]     INT          NULL,
    [paymentDate] DATE         NULL,
    [validTill]   DATE         NULL,
    FOREIGN KEY ([customerId]) REFERENCES [dbo].[CheckOutCustomer] ([CustomerId])
);

