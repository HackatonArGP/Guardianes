CREATE TABLE [dbo].[Comments] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [EarthwatcherId] INT            NOT NULL,
    [LandId]         INT            NOT NULL,
    [UserComment]    NVARCHAR (255) NOT NULL,
    [Published]      DATETIME2 (7)  NOT NULL,
    [IsDeleted]      BIT            CONSTRAINT [DF_Comments_IsDeleted] DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IDXCommentEarthwatcherId]
    ON [dbo].[Comments]([EarthwatcherId] ASC);


GO
CREATE NONCLUSTERED INDEX [IDXCommentLandId]
    ON [dbo].[Comments]([LandId] ASC);

