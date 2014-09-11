CREATE PROCEDURE [dbo].[SatelliteImage_Delete] 
@id INT
AS
BEGIN
delete from SatelliteImage where id=@id
END
