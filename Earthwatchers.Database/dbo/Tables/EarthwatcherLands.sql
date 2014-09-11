CREATE TABLE [dbo].[EarthwatcherLands] (
    [Land]         INT           NOT NULL,
    [Earthwatcher] INT           NOT NULL,
    [AddedDate]    SMALLDATETIME CONSTRAINT [DF_EarthwatcherLands_AddedDate] DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_EarthwatcherLands_1] PRIMARY KEY CLUSTERED ([Land] ASC),
    CONSTRAINT [FK_EarthwatcherLands_Earthwatcher] FOREIGN KEY ([Earthwatcher]) REFERENCES [dbo].[Earthwatcher] ([Id]),
    CONSTRAINT [FK_EarthwatcherLands_Land] FOREIGN KEY ([Land]) REFERENCES [dbo].[Land] ([Id])
);

