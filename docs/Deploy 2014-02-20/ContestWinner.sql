ALTER TABLE dbo.Contest ADD
	WinnerId int NULL
	
Update Contest
Set WinnerId = (
	Select TOP 1 s.EarthwatcherId From scores s
	Inner Join Earthwatcher e on s.EarthwatcherId = e.Id
	Inner Join Contest c on c.Id = 1
	Where Published BETWEEN c.StartDate and c.EndDate
	Group by s.EarthwatcherId, e.Name
	ORDER BY SUM(points) DESC)
Where Id = 1