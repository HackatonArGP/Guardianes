CREATE PROCEDURE [dbo].[Jaguar_Get] 

AS
BEGIN
Select Id, Day, Hour, Minutes, Point.Lat as Latitude, Point.Long as Longitude, FoundBy from JaguarPositions
END
