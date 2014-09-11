CREATE PROCEDURE [dbo].[Land_VerificationScoring_3] 
@LandId INT
AS
BEGIN
Update EarthwatcherLands Set Earthwatcher = (Select Id From Earthwatcher where Name = 'greenpeace@greenpeace.org'), AddedDate = GETUTCDATE() Where Land = @LandId
                    Update Land Set IsLocked = 1 Where Id = @LandId
END
