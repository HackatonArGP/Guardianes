CREATE PROCEDURE [dbo].[Collections_GetTotalItems] 
@earthwatcherId INT
AS
BEGIN
select count(*) from EarthwatcherCollections where EarthwatcherId = @earthwatcherId
END
