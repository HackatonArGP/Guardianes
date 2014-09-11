CREATE PROCEDURE [dbo].[Land_AssignLandToEarthwatcher]
@GeohexKey VARCHAR(11),
@EarthwatcherId INT
AS
BEGIN
update Land set StatusChangedDateTime = GETUTCDATE(), LandStatus = 2 where GeohexKey = @GeohexKey
Delete From EarthwatcherLands Where Land in (Select Id From Land Where GeoHexKey = @GeohexKey)
INSERT INTO EarthwatcherLands (Land, Earthwatcher) VALUES ((Select Id From Land Where GeoHexKey = @GeohexKey), @EarthwatcherId)
END
