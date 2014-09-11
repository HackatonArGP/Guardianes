CREATE PROCEDURE [dbo].[SatelliteImage_GetAll] 

AS
BEGIN
select Id, extent.STAsText() as Wkt, Name, Provider,Published,AcquisitionDate, ImageType, UrlTileCache, MinLevel, MaxLevel, UrlMetadata, 
	   extent.STStartPoint().Lat ymin, extent.STStartPoint().Long xmin, extent.STPointN(3).Lat ymax, extent.STPointN(3).Long xmax, IsCloudy 
from SatelliteImage 
Where Name = '2008'
                
UNION 

Select * from (select top 1000 Id, extent.STAsText() as Wkt, Name, Provider, Published, AcquisitionDate, ImageType, UrlTileCache, MinLevel, MaxLevel, UrlMetadata, 
		extent.STStartPoint().Lat ymin, extent.STStartPoint().Long xmin, extent.STPointN(3).Lat ymax, extent.STPointN(3).Long xmax, IsCloudy 
		from SatelliteImage Order By Published DESC) lastImages
END
