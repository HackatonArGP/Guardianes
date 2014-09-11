CREATE TABLE [dbo].[PollResults] (
    [Land]             INT           NOT NULL,
    [Earthwatcher]     INT           NOT NULL,
    [HasDeforestation] BIT           CONSTRAINT [DF_PollResults_HasDeforestation] DEFAULT ((0)) NOT NULL,
    [PollDate]         SMALLDATETIME CONSTRAINT [DF_PollResults_PollDate] DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_PollResults] PRIMARY KEY CLUSTERED ([Land] ASC, [Earthwatcher] ASC)
);

