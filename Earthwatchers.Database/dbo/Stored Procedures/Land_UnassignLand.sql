CREATE PROCEDURE [dbo].[Land_UnassignLand] 
@Id INT
AS
BEGIN
update land set LandStatus = 1 where Id = @Id
                Delete From EarthwatcherLands Where Land = @Id
END
