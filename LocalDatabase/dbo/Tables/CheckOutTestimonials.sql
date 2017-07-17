CREATE TABLE [dbo].[CheckOutTestimonials] (
    [CustomerId]       VARCHAR (10)  NOT NULL,
    [CustomerName]     VARCHAR (50)  NOT NULL,
    [CustomerLiked]    VARCHAR (50)  NOT NULL,
    [CustomerFrom]     VARCHAR (50)  NOT NULL,
    [CustomerWords]    VARCHAR (500) NOT NULL,
    [CustomerStar]     INT           NOT NULL,
    [CustomerFacebook] VARCHAR (200) NOT NULL,
    [CustomerImage]    VARCHAR (150) NULL,
    FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[CheckOutCustomer] ([CustomerId])
);

