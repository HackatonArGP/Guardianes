CREATE TYPE [dbo].[LandsCreateType] AS TABLE(
	[GeohexKey] [nvarchar](11) NULL,
	[Confirmed] [bit] NULL,
	[LandStatus] [int] NULL,
	[Name] [nvarchar](255) NULL,
	[Observations] [nvarchar](255) NULL,
	[Lat] [float] NULL,
	[Long] [float] NULL,
	[BasecampId] [int] NULL,
	[LandThreat] [int]
)
GO