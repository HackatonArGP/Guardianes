CREATE PROCEDURE [dbo].[Land_ReassignLand] 
@status INT,
@newGeoHexKey VARCHAR(11),
@currentLand INT,
@EarthwatcherId INT
AS
BEGIN
IF @status > 2 -- Pasa a Greenpeace
	                                BEGIN
		                                Update EarthwatcherLands Set Earthwatcher = (Select Id From Earthwatcher Where Name = 'greenpeace@greenpeace.org') Where Land = @currentLand
	                                END
                                ELSE
	                                BEGIN
		                                DELETE FROM EarthwatcherLands Where Land = @currentLand
		                                Update Land set LandStatus = 1, StatusChangedDateTime = GETUTCDATE() where Id = @currentLand
	                                END

                                DELETE FROM EarthwatcherLands Where Land in (Select Id From Land Where GeoHexKey = @newGeoHexKey)
                                INSERT INTO EarthwatcherLands (Land, Earthwatcher) VALUES ((Select Id From Land Where GeoHexKey = @newGeoHexKey), @EarthwatcherId)
                                Update Land set LandStatus = 2, StatusChangedDateTime = GETUTCDATE() Where GeoHexKey = @newGeoHexKey
END
