CREATE PROCEDURE [dbo].[Land_ReassignLandByEarthwatcherId] 
@newGeoHexKey VARCHAR(11),
@EarthwatcherId INT
AS
BEGIN

declare @currentLand int = (select top 1 Land from EarthwatcherLands where Earthwatcher = @EarthwatcherId) --Guardo el Id de la land que el usuario quiere cambiar
declare @status int = (select top 1 LandStatus from Land where Id = @currentLand)  --Guardo el Status de la land que quiene cambiar

IF @status > 2 -- Si la reviso pasa a Greenpeace
	BEGIN
		Update EarthwatcherLands Set Earthwatcher = (Select Id From Earthwatcher Where Name = 'greenpeace@greenpeace.org') Where Land = @currentLand
	END
ELSE  --Si no la reviso pone estado 1 sin asignar y la borra de ese usuario
    BEGIN
        DELETE FROM EarthwatcherLands Where Land = @currentLand
        Update Land set LandStatus = 1, StatusChangedDateTime = GETUTCDATE() where Id = @currentLand
    END

DELETE FROM EarthwatcherLands Where Land in (Select Id From Land Where GeoHexKey = @newGeoHexKey)  --Borra la nueva land que se le asigno al que la tenga si es que alguno la tiene asignada(robada)
INSERT INTO EarthwatcherLands (Land, Earthwatcher) VALUES ((Select Id From Land Where GeoHexKey = @newGeoHexKey), @EarthwatcherId)  --Le asigna la nueva land al usuario
Update Land set LandStatus = 2, StatusChangedDateTime = GETUTCDATE() Where GeoHexKey = @newGeoHexKey   --Le pone status 2 sin revisar a la nueva land del usuario

END