CREATE PROCEDURE [dbo].[Jaguar_GetPos] 
@id INT
AS
BEGIN
Select Id, Day, Hour, Minutes, Point.Lat as Latitude, Point.Long as Longitude, FoundBy from JaguarPositions where Id = @id
END
