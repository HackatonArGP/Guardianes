CREATE NONCLUSTERED INDEX IX_NC_PolygonId
ON dbo.Locations (PolygonId)
INCLUDE (Id, Latitude, Longitude,[Index])


CREATE NONCLUSTERED INDEX IX_NC_ZoneId
ON dbo.Polygons (ZoneId)
INCLUDE (Id, Name,PolygonGeom)