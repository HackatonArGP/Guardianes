CREATE PROCEDURE [dbo].[Earthwatcher_LinkApiAndEarthwatcher] 
@ApiEwId int,
@EwId int
AS
BEGIN
update ApiEwLogin set EarthwatcherId = @EwId where Id = @ApiEwId
update Earthwatcher set ApiEwId = @ApiEwId where Id = @EwId
END