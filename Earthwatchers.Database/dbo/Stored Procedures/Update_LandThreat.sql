
CREATE PROCEDURE [dbo].[Update_LandThreat]
AS
BEGIN
	
	  --reset lands to NO thread
  Update Land
  Set LandThreat = 0

  --correct geometry type
  UPDATE Polygons
  SET PolygonGeom = PolygonGeom.MakeValid();  
  UPDATE Polygons
  SET PolygonGeom = PolygonGeom.STUnion(PolygonGeom.STStartPoint());   
  UPDATE Polygons
  SET PolygonGeom = PolygonGeom.MakeValid().STUnion(PolygonGeom.STStartPoint());

--update land thread intersecting with polygons
update Land
set LandThreat = case when t.ZoneName = 'Zona Roja' then 5 else 3 end
from Land l, 
		(select GEOGRAPHY::STGeomFromText(p.PolygonGeom.STAsText(),4326) as Polygon,
		z.Name as ZoneName
		from Polygons p
		inner join Zones z on p.ZoneId = z.Id
		where z.Name in ('Zona Roja', 'Zona Amarilla')) as t
where l.Centroid.STIntersects(t.Polygon)=1

	
END

