CREATE TABLE [dbo].[CollectionItems] (
    [Id]           INT          IDENTITY (1, 1) NOT NULL,
    [CollectionId] INT          NOT NULL,
    [Name]         VARCHAR (50) NOT NULL,
    [Icon]         VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_CollectionItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CollectionItems_Collections] FOREIGN KEY ([CollectionId]) REFERENCES [dbo].[Collections] ([Id])
);

