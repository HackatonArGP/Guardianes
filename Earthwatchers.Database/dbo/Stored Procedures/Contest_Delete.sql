Create PROCEDURE [dbo].[Contest_Delete] 
@id INT
AS
BEGIN
delete from Contest where id=@id
END
