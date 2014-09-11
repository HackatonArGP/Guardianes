CREATE PROCEDURE [dbo].[Basecamp_GetBaseCamps] 
AS
BEGIN
Select Id, Name, CAST(0 AS float) Latitude, CAST(0 AS float) Longitude, 0 Probability, '' DetailName From Basecamp order by Name
END
