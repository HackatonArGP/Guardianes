CREATE TABLE [dbo].[SatelliteImage] (
    [Id]              INT               IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (255)    NOT NULL,
    [Provider]        NVARCHAR (255)    NOT NULL,
    [Extent]          [sys].[geography] NOT NULL,
    [Published]       DATETIME2 (7)     NULL,
    [ImageType]       INT               NOT NULL,
    [UrlTileCache]    NVARCHAR (255)    NOT NULL,
    [UrlMetadata]     NVARCHAR (255)    NULL,
    [MinLevel]        INT               DEFAULT ((0)) NULL,
    [MaxLevel]        INT               DEFAULT ((12)) NULL,
    [AcquisitionDate] DATETIME2 (7)     DEFAULT (getdate()) NULL,
    [IsCloudy]        BIT               CONSTRAINT [DF_SatelliteImage_IsCloudy] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PrimaryKeyConstraintSatelliteImage] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE SPATIAL INDEX [SpatialIndexSatelliteImageExtent]
    ON [dbo].[SatelliteImage] ([Extent])
    USING GEOGRAPHY_GRID
    WITH  (
            GRIDS = (LEVEL_1 = MEDIUM, LEVEL_2 = MEDIUM, LEVEL_3 = MEDIUM, LEVEL_4 = MEDIUM)
          );

