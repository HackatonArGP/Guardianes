CREATE PROCEDURE [dbo].[Zone_Delete]
	@ZoneId int
AS
BEGIN
	DELETE FROM Locations WHERE PolygonId in (select Id from Polygons where ZoneId = @ZoneId)
	
	DELETE FROM Polygons WHERE ZoneId = @ZoneId
	
	DELETE FROM Zones WHERE Id = @ZoneId

END
