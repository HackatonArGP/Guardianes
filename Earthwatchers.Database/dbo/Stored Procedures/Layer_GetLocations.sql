CREATE PROCEDURE [dbo].[Layer_GetLocations] 
@polygonId INT
AS
BEGIN
select * from Locations where PolygonId = @polygonId
END
