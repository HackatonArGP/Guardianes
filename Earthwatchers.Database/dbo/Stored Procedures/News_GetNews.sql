CREATE PROCEDURE [dbo].[News_GetNews] 

AS
BEGIN
select n.Id,Shape.STAsText() as Wkt,e.Name as Username, EarthwatcherId,Published,NewsItem from News n left join Earthwatcher e on n.EarthwatcherId=e.Id order by n.Published desc
END
