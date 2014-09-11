
CREATE PROCEDURE [dbo].[Lands_Reassign] 
	@lands ReassignType READONLY
AS
BEGIN
	SET NOCOUNT ON;

    -- Desasigno todas las lands de los Earthwatchers
    Update Land
    Set LandStatus = 1, LastReset = GETUTCDATE(), StatusChangedDateTime = GETUTCDATE()
    From Land 
    Where Id in (Select Land From EarthwatcherLands)
    
    Delete From EarthwatcherLands
    
    --Asigno la nueva Parcela
    Update Land
    Set LandStatus = 2, LastReset = GETUTCDATE(), StatusChangedDateTime = GETUTCDATE()
    From Land
    Where GeoHexKey in (Select distinct GeoHexKey From @lands)
    
    INSERT INTO EarthwatcherLands (Land, Earthwatcher)
    Select land.Id, l.EarthwatcherId From @lands l
    Inner Join Land land on l.GeoHexKey = land.GeoHexKey
END

