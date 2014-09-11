CREATE TABLE [dbo].[EarthwatcherCollections] (
    [EarthwatcherId]   INT           NOT NULL,
    [CollectionItemId] INT           NOT NULL,
    [AddedDate]        SMALLDATETIME CONSTRAINT [DF_EarthwatcherCollections_AddedDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [FK_EarthwatcherCollections_CollectionItems] FOREIGN KEY ([CollectionItemId]) REFERENCES [dbo].[CollectionItems] ([Id]),
    CONSTRAINT [FK_EarthwatcherCollections_Earthwatcher] FOREIGN KEY ([EarthwatcherId]) REFERENCES [dbo].[Earthwatcher] ([Id])
);

