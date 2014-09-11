CREATE TABLE [dbo].[Zones] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50)  NOT NULL,
    [Description] VARCHAR (200) NULL,
    [LayerId]     INT           NOT NULL,
    [Param1]      VARCHAR (50)  NULL,
    CONSTRAINT [PK_Zones] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Layers_Zones] FOREIGN KEY ([LayerId]) REFERENCES [dbo].[Layers] ([Id])
);
GO

