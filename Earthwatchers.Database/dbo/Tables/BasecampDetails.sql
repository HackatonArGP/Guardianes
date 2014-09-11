CREATE TABLE [dbo].[BasecampDetails] (
    [Id]          INT               IDENTITY (1, 1) NOT NULL,
    [BasecampId]  INT               NOT NULL,
    [Location]    [sys].[geography] NOT NULL,
    [Probability] INT               CONSTRAINT [DF_BasecampDetails_Probability] DEFAULT ((10)) NOT NULL,
    [Name]        VARCHAR (200)     NOT NULL,
    [ShortText]   VARCHAR (MAX)     CONSTRAINT [DF_BasecampDetails_ShortText] DEFAULT ('') NULL,
    CONSTRAINT [PK_BasecampDetails] PRIMARY KEY CLUSTERED ([Id] ASC)
);



