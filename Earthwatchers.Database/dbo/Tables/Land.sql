CREATE TABLE [dbo].[Land] (
    [Id]                    INT               IDENTITY (1, 1) NOT NULL,
    [Centroid]              [sys].[geography] NOT NULL,
    [GeohexKey]             NVARCHAR (11)     NOT NULL,
    [LandType]              INT               NOT NULL,
    [LandThreat]            INT               NOT NULL,
    [Distance]              FLOAT (53)        NULL,
    [LandStatus]            INT               NOT NULL,
    [AlertDateTime]         DATETIME2 (7)     NULL,
    [StatusChangedDateTime] DATETIME2 (7)     NULL,
    [DemandAuthorities]     BIT               CONSTRAINT [DF_Land_DemandAuthorities] DEFAULT ((0)) NOT NULL,
    [DemandUrl]             VARCHAR (100)     NULL,
    [LastReset]             SMALLDATETIME     CONSTRAINT [DF_Land_LastReset] DEFAULT (getutcdate()) NOT NULL,
    [IsLocked]              BIT               CONSTRAINT [DF_Land_IsLocked] DEFAULT ((0)) NOT NULL,
    [Confirmed]             BIT               NULL,
    [Latitude]              FLOAT (53)        NULL,
    [Longitude]             FLOAT (53)        NULL,
    [ConfirmedBy]           INT               NULL,
    [ReportResetted]        SMALLDATETIME     NULL,
    [BasecampId]            INT               NULL,
    CONSTRAINT [PrimaryKeyConstraintLand] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
