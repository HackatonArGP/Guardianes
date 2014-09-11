CREATE TABLE [dbo].[BasecampLandDistance] (
    [Id]               INT        IDENTITY (1, 1) NOT NULL,
    [BasecampDetailId] INT        NOT NULL,
    [LandId]           INT        NOT NULL,
    [Distance]         FLOAT (53) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([BasecampDetailId]) REFERENCES [dbo].[BasecampDetails] ([Id]),
    CONSTRAINT [FK__BasecampL__LandI__2DE6D218] FOREIGN KEY ([LandId]) REFERENCES [dbo].[Land] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_NC_BasecampLandDistance]
    ON [dbo].[BasecampLandDistance]([BasecampDetailId] ASC)
    INCLUDE([LandId], [Distance]);


GO
CREATE NONCLUSTERED INDEX [IX_NC_BasecampLandDistance_LandId]
    ON [dbo].[BasecampLandDistance]([LandId] ASC)
    INCLUDE([BasecampDetailId], [Distance]);

