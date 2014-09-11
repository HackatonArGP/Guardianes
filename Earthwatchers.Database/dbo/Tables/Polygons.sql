CREATE TABLE [dbo].[Polygons] (
    [Id]          INT              IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (50)     NOT NULL,
    [ZoneId]      INT              NOT NULL,
    [PolygonGeom] [sys].[geometry] NULL,
    CONSTRAINT [PK_Polygons] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Zones_Polygons] FOREIGN KEY ([ZoneId]) REFERENCES [dbo].[Zones] ([Id])
);

