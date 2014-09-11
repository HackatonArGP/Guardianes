CREATE PROCEDURE [dbo].[Basecamp_Get] 
AS
BEGIN
Select d.Id, b.Name, d.Location.Lat as Latitude, d.Location.Long as Longitude, d.Probability, d.Name DetailName, d.ShortText From Basecamp b Inner Join BasecampDetails d on b.Id = d.basecampId order by b.Name
END
