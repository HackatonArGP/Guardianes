CREATE TABLE [dbo].[Settings] (
    [Name] NVARCHAR (255) NOT NULL,
    [Val]  NVARCHAR (255) NOT NULL,
    CONSTRAINT [PrimaryKeyConstraintSettings] PRIMARY KEY CLUSTERED ([Name] ASC)
);

