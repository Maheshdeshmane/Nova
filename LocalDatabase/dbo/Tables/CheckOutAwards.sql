CREATE TABLE [dbo].[CheckOutAwards] (
    [CustomerId]    VARCHAR (10) NOT NULL,
    [FacebookLikes] INT          NOT NULL,
    [HappyClients]  INT          NOT NULL,
    [Awards]        INT          NOT NULL,
    [Followers]     INT          NOT NULL,
    FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[CheckOutCustomer] ([CustomerId])
);

