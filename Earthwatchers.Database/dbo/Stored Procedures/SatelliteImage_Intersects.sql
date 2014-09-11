CREATE PROCEDURE [dbo].[SatelliteImage_Intersects] 
@wkt VARCHAR(255)
AS
BEGIN
select Id,extent.STAsText() as Wkt, Name,Provider,Published,AcquisitionDate,ImageType, UrlTileCache, MinLevel, MaxLevel, UrlMetadata, extent.STStartPoint().Lat ymin, extent.STStartPoint().Long xmin, extent.STPointN(3).Lat ymax, extent.STPointN(3).Long xmax, IsCloudy from SatelliteImage 
             where extent.STIntersects(geography::STGeomFromText(@wkt, 4326))=1 Order By Published DESC
END
