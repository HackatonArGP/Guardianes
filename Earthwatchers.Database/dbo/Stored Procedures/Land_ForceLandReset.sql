CREATE PROCEDURE [dbo].[Land_ForceLandReset] 
 @LandId int,
 @EarthwatcherId int
AS
BEGIN
	--Clear history
	UPDATE Verifications	
	SET IsDeleted = 1
	WHERE Land = @LandId
		
	UPDATE Comments	
	SET IsDeleted = 1
	WHERE LandId = @LandId

	--Unassing land from earthwatcher
	DELETE FROM EarthwatcherLands WHERE Earthwatcher = @EarthwatcherId

	--Reset land values
	UPDATE Land 
	SET LandStatus = 1
	  , LastReset = GETUTCDATE()
	  , DemandAuthorities = 0
	  , Confirmed = null
	  , ConfirmedBy = null
	  , IsLocked = 0
	WHERE Id = @LandId
END

