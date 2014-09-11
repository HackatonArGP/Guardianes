CREATE TABLE [dbo].[Flags] (
    [Id]             INT               IDENTITY (1, 1) NOT NULL,
    [EarthwatcherId] INT               NOT NULL,
    [Location]       [sys].[geography] NOT NULL,
    [Comment]        NVARCHAR (255)    NULL,
    [Published]      DATETIME2 (7)     NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

