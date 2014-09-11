CREATE PROCEDURE [dbo].[Earthwatcher_DeleteEarthwatcher] 
@id INT
AS
BEGIN
				delete from Verifications where Earthwatcher=@id
                delete from Comments where EarthwatcherId=@id
                delete from EarthwatcherLands where Earthwatcher=@id
                delete from Earthwatcher where id=@id
END
