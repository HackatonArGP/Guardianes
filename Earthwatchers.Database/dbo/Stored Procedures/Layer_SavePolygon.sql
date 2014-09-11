CREATE PROCEDURE [dbo].[Layer_SavePolygon] 
@name VARCHAR(50),
@ID INT Output,
@zoneId INT,
@Polygon GEOMETRY
AS
BEGIN
insert into Polygons(Name,ZoneId, PolygonGeom) values(@name,@zoneId,@Polygon)SET @ID = SCOPE_IDENTITY()
END
