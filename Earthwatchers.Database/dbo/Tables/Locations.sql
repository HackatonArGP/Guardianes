CREATE TABLE [dbo].[Locations] (
    [Id]        INT        IDENTITY (1, 1) NOT NULL,
    [Latitude]  FLOAT (53) NOT NULL,
    [Longitude] FLOAT (53) NOT NULL,
    [Index]     INT        NOT NULL,
    [PolygonId] INT        NOT NULL,
    CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Polygons_Locations] FOREIGN KEY ([PolygonId]) REFERENCES [dbo].[Polygons] ([Id])
);

