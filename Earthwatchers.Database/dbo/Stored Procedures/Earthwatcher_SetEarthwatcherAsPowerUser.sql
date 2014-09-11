CREATE PROCEDURE [dbo].[Earthwatcher_SetEarthwatcherAsPowerUser] 
@id INT,
@isPowerUser BIT = 0
AS
BEGIN
update Earthwatcher set IsPowerUser=@isPowerUser where id=@id
END
