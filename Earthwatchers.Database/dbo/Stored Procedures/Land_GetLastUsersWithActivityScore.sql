CREATE PROCEDURE [dbo].[Land_GetLastUsersWithActivityScore] 
@landId INT
AS
BEGIN
Select CAST(ROW_NUMBER() OVER(ORDER BY AddedDate ASC) AS INT) AS Id, [action] = SUBSTRING(Name, 0, CHARINDEX('@', Name)), EarthwatcherId = Earthwatcher, published = AddedDate, 0 points From Verifications d Inner Join Earthwatcher e on d.Earthwatcher = e.Id Where d.Land = @landId and d.IsDeleted = 0 Order by AddedDate ASC
END
