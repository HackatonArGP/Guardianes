CREATE TABLE [dbo].[Earthwatcher] (
    [Id]               INT              IDENTITY (1, 1) NOT NULL,
    [EarthwatcherGuid] UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Role]             INT              CONSTRAINT [DF__Earthwatch__Role__30F848ED] DEFAULT ((0)) NOT NULL,
    [PasswordPrefix]   NVARCHAR (255)   NULL,
    [hashedpassword]   NVARCHAR (255)   NULL,
    [country]          NVARCHAR (255)   NULL,
    [avatar]           NVARCHAR (255)   NULL,
    [FullName]         NVARCHAR (255)   NULL,
    [email]            NVARCHAR (255)   NULL,
    [Telephone]        NVARCHAR (255)   NULL,
    [IsPowerUser]      BIT              CONSTRAINT [DF_Earthwatcher_IsPowerUser] DEFAULT ((0)) NOT NULL,
    [Language]         NVARCHAR (255)   NULL,
    [Region]           NVARCHAR (255)   NULL,
    [NotifyMe]         BIT              CONSTRAINT [DF_Earthwatcher_NotifyMe] DEFAULT ((1)) NOT NULL,
    [NickName]         NVARCHAR (50)    NULL,
    [CreatedDate]      SMALLDATETIME    CONSTRAINT [DF_Earthwatcher_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [ApiEwId]          INT              NULL,
    CONSTRAINT [PrimaryKeyConstraintEarthwatcher] PRIMARY KEY CLUSTERED ([Id] ASC)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [EarthwatcherGuidUnique]
    ON [dbo].[Earthwatcher]([EarthwatcherGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IDXEarthwatcherName]
    ON [dbo].[Earthwatcher]([Name] ASC);

