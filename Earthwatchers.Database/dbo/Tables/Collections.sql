CREATE TABLE [dbo].[Collections] (
    [Id]              INT          IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (50) NOT NULL,
    [BackgroundImage] VARCHAR (50) NULL,
    CONSTRAINT [PK_Collections] PRIMARY KEY CLUSTERED ([Id] ASC)
);

