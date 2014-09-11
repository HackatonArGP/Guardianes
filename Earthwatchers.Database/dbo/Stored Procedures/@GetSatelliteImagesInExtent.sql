CREATE PROCEDURE [dbo].[@GetSatelliteImagesInExtent]
	@GeometryString NVARCHAR(MAX)
AS

    DECLARE  @QueryGeometry GEOGRAPHY
    
    SET @QueryGeometry = GEOGRAPHY::STGeomFromText(@GeometryString, 4326);

    SELECT Id, Name, Provider, Extent, Published, ImageType
    FROM [SatelliteImage] 
    WHERE @QueryGeometry.STIntersects([Extent]) = 1

RETURN
