CREATE PROCEDURE [dbo].[Earthwatcher_EarthwatcherExists] 
@name VARCHAR(255)
AS
BEGIN
	Select Id From Earthwatcher Where name = @name
END
