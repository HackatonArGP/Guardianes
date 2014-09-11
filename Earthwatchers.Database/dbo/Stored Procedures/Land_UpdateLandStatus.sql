CREATE PROCEDURE [dbo].[Land_UpdateLandStatus] 
@LandStatus INT,
@StatusChangedDateTime DATETIME2(7),
@Id INT
AS
BEGIN
update land set LandStatus=@LandStatus, StatusChangedDateTime=@StatusChangedDateTime where Id=@Id
END
