USE [Earthwatchers]
GO

/****** Object:  Table [dbo].[PopupMessages]    Script Date: 02/04/2014 14:40:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PopupMessages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShortTitle] [varchar](50) NULL,
	[Title] [varchar](100) NOT NULL,
	[Description] [varchar](500) NOT NULL,
	[ImageURL] [varchar](100) NULL,
	[StartDate] [smalldatetime] NOT NULL,
	[EndDate] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_PopupMessages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


