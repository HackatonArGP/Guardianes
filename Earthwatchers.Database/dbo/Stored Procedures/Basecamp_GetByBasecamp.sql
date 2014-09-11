CREATE PROCEDURE [dbo].[Basecamp_GetByBasecamp] 
	@Name VARCHAR(200)
AS
BEGIN
Select d.Id, d.BasecampId, b.Name, d.Location.Lat as Latitude, d.Location.Long as Longitude, d.Probability, d.Name DetailName, d.ShortText From Basecamp b Inner Join BasecampDetails d on b.Id = d.basecampId where d.Probability > 0 and b.Name = @Name order by d.Probability 
END
