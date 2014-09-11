CREATE PROCEDURE [dbo].[Land_GetAllLands] 

AS
BEGIN
Select Id, Latitude, Longitude, GeohexKey, Distance, LandThreat from Land
END
