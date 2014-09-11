CREATE TYPE [dbo].[LandsType] AS TABLE (
    [GeohexKey]    NVARCHAR (11)  NULL,
    [Confirmed]    BIT            NULL,
    [LandStatus]   INT            NULL,
    [Name]         NVARCHAR (255) NULL,
    [Observations] NVARCHAR (255) NULL,
    [Lat]          FLOAT (53)     NULL,
    [Long]         FLOAT (53)     NULL);

