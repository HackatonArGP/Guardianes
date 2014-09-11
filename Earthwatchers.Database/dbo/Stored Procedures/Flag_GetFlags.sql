CREATE PROCEDURE [dbo].[Flag_GetFlags] 

AS
BEGIN
select l.Id as Id, l.EarthwatcherId as EarthwatcherId, l.Location.Lat as Latitude, l.Location.Long as Longitude, l.Comment as Comment, Published, e.Name as UserName from Flags l left join Earthwatcher e on l.EarthwatcherId=e.Id
END
