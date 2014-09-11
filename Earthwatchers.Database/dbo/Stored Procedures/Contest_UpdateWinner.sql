CREATE PROCEDURE [dbo].[Contest_UpdateWinner] 
AS
BEGIN
	SET NOCOUNT ON;

	Update Contest
	Set WinnerId = (
		Select TOP 1 s.EarthwatcherId From scores s
		Inner Join Earthwatcher e on s.EarthwatcherId = e.Id
		Inner Join Contest c on c.WinnerId is null and c.EndDate <= GETDATE()
		Where Published BETWEEN c.StartDate and c.EndDate
		Group by s.EarthwatcherId, e.Name
		ORDER BY SUM(points) DESC)
	Where WinnerId is null and EndDate <= GETDATE()
		
END
