CREATE PROCEDURE [dbo].[Update_LandBasecamp]
AS
BEGIN
  --correct geometry type
  UPDATE Polygons
  SET PolygonGeom = PolygonGeom.MakeValid();  
  UPDATE Polygons
  SET PolygonGeom = PolygonGeom.STUnion(PolygonGeom.STStartPoint());   
  UPDATE Polygons
  SET PolygonGeom = PolygonGeom.MakeValid().STUnion(PolygonGeom.STStartPoint());

--update BasecampId intersecting with polygons
update Land
set BasecampId = case when t.BcId > 0 then t.BcId else null end
from Land l, 
		(select GEOGRAPHY::STGeomFromText(p.PolygonGeom.STAsText(),4326) as Polygon,
		z.Param1 as BcId
		from Polygons p
		inner join Zones z on p.ZoneId = z.Id
		where z.LayerId = (select Id from Layers where Name = 'FincasLayer')) as t
where l.Centroid.STIntersects(t.Polygon)= 1

END