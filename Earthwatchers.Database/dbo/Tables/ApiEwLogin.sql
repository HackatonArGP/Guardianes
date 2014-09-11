CREATE TABLE [dbo].[ApiEwLogin] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [UserId]         VARCHAR (255) NOT NULL,
    [NickName]       VARCHAR (255) NULL,
    [AccessToken]    VARCHAR (MAX) NOT NULL,
    [SecretToken]    VARCHAR (255) NULL,
    [Api]            VARCHAR (50)  NOT NULL,
    [EarthwatcherId] INT           NULL,
    [Mail]           VARCHAR (255) NULL,
    CONSTRAINT [PK_ApiEwLogin] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApiEwLogin_ApiEwLogin] FOREIGN KEY ([Id]) REFERENCES [dbo].[ApiEwLogin] ([Id])
);



