CREATE TABLE [dbo].[ThreatLevel] (
    [Id]          INT               IDENTITY (1, 1) NOT NULL,
    [Shape]       [sys].[geography] NOT NULL,
    [ThreatLevel] INT               NOT NULL,
    CONSTRAINT [PrimaryKeyConstraintThreatLevel] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE SPATIAL INDEX [ThreatLevelShape]
    ON [dbo].[ThreatLevel] ([Shape])
    USING GEOGRAPHY_GRID
    WITH  (
            GRIDS = (LEVEL_1 = MEDIUM, LEVEL_2 = MEDIUM, LEVEL_3 = MEDIUM, LEVEL_4 = MEDIUM)
          );

