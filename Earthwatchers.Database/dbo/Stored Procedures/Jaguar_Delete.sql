CREATE PROCEDURE [dbo].[Jaguar_Delete] 
@id INT
AS
BEGIN
delete from JaguarPositions where id=@id
END
