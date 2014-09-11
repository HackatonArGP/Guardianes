CREATE TABLE [dbo].[JaguarPositions] (
    [Id]      INT               IDENTITY (1, 1) NOT NULL,
    [Day]     INT               NOT NULL,
    [Hour]    INT               NOT NULL,
    [Minutes] INT               NOT NULL,
    [Point]   [sys].[geography] NOT NULL,
    [FoundBy] VARCHAR (255)     NULL,
    CONSTRAINT [PK_JaguarsPosition] PRIMARY KEY CLUSTERED ([Id] ASC)
);

