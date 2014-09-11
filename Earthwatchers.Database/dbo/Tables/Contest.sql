CREATE TABLE [dbo].[Contest] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [ShortTitle]  VARCHAR (50)  NULL,
    [Title]       VARCHAR (100) NOT NULL,
    [Description] VARCHAR (500) NULL,
    [StartDate]   SMALLDATETIME NOT NULL,
    [EndDate]     SMALLDATETIME NOT NULL,
    [ImageURL]    VARCHAR (100) NULL,
    [WinnerId]    INT           NULL,
    CONSTRAINT [PK_Contest2] PRIMARY KEY CLUSTERED ([Id] ASC)
);

