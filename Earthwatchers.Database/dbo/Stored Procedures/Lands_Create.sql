CREATE PROCEDURE [dbo].[Lands_Create] 
	@lands LandsCreateType READONLY
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO Land (Centroid, Geohexkey, LandThreat, Distance, LandStatus, LandType, Latitude, Longitude, BasecampId)
    Select geography::Point(Lat, Long, 4326), Geohexkey, LandThreat, 0, 1, -1, Lat, Long, BasecampId From @lands
   
END
