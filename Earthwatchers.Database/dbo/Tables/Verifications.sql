CREATE TABLE [dbo].[Verifications] (
    [Land]         INT      NOT NULL,
    [Earthwatcher] INT      NOT NULL,
    [IsAlert]      BIT      CONSTRAINT [DF_Verifications_IsOk] DEFAULT ((0)) NOT NULL,
    [AddedDate]    DATETIME CONSTRAINT [DF_Verifications_AddedDate] DEFAULT (getutcdate()) NOT NULL,
    [IsDeleted]    BIT      CONSTRAINT [DF_Verifications_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Verifications] PRIMARY KEY CLUSTERED ([Land] ASC, [Earthwatcher] ASC)
);



