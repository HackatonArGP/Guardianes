CREATE TABLE [dbo].[scores] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [EarthwatcherId] INT            NOT NULL,
    [action]         NVARCHAR (255) NOT NULL,
    [published]      DATETIME2 (7)  NOT NULL,
    [points]         INT            NOT NULL,
    [LandId]         INT            NULL,
    [Param1]         NVARCHAR (50)  NULL,
    [Param2]         NVARCHAR (50)  NULL,
    CONSTRAINT [PK_scores] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);




GO
CREATE CLUSTERED INDEX [cdxId]
    ON [dbo].[scores]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [cdxEwId]
    ON [dbo].[scores]([EarthwatcherId] ASC);

