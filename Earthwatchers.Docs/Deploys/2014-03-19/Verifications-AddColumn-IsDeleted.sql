Use Earthwatchers
GO

ALTER TABLE dbo.Verifications ADD
	IsDeleted bit NOT NULL CONSTRAINT DF_Verifications_IsDeleted DEFAULT 0
GO