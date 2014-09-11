CREATE TABLE [dbo].[News] (
    [Id]             INT               IDENTITY (1, 1) NOT NULL,
    [Shape]          [sys].[geography] NOT NULL,
    [EarthwatcherId] INT               NOT NULL,
    [Published]      DATETIME2 (7)     NOT NULL,
    [NewsItem]       NVARCHAR (MAX)    NULL,
    CONSTRAINT [PrimaryKeyConstraintNews] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE SPATIAL INDEX [NewsShape]
    ON [dbo].[News] ([Shape])
    USING GEOGRAPHY_GRID
    WITH  (
            GRIDS = (LEVEL_1 = MEDIUM, LEVEL_2 = MEDIUM, LEVEL_3 = MEDIUM, LEVEL_4 = MEDIUM)
          );

