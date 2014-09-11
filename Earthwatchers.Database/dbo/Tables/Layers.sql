CREATE TABLE [dbo].[Layers] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50)  NOT NULL,
    [Description] VARCHAR (200) NULL,
    CONSTRAINT [PK_Layer] PRIMARY KEY CLUSTERED ([Id] ASC)
);

