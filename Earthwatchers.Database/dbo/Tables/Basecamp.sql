CREATE TABLE [dbo].[Basecamp] (
    [Id]    INT               IDENTITY (1, 1) NOT NULL,
    [Shape] [sys].[geography] NOT NULL,
    [Name]  NVARCHAR (255)    NOT NULL,
    CONSTRAINT [PrimaryKeyConstraintBasecamp] PRIMARY KEY CLUSTERED ([Id] ASC)
);

