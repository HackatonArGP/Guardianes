CREATE PROCEDURE [dbo].[Layer_GetPolygons] 
@zoneId INT
AS
BEGIN
select Id, Name, ZoneId from Polygons where ZoneId = @zoneId
END
