CREATE PROCEDURE [dbo].[Land_MassiveReassign_sql] 

AS
BEGIN
select l.Id, l.Latitude, l.Longitude, GeohexKey, Distance, LandThreat, LandStatus, StatusChangedDateTime, DemandAuthorities, DemandUrl, LastReset, el.Earthwatcher from Land l inner join EarthwatcherLands el on l.Id = el.Land Where LandStatus != 4 and l.GeohexKey <> 'NZ637648' --Parcela del tutor no se puede reshuffle
END
