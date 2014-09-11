CREATE PROCEDURE [dbo].[Basecamp_Delete] 
@id INT
AS
BEGIN
delete from BasecampLandDistance where BasecampDetailId = @id
delete from BasecampDetails where id= @id
END
