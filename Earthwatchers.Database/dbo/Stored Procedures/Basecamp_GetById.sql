Create PROCEDURE [dbo].[Basecamp_GetById] 
@id int
AS
BEGIN
Select BasecampId, Name, Location.Lat as Latitude, Location.Long as Longitude, Probability, ShortText  From BasecampDetails where Id = @id;
END
