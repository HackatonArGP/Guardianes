CREATE PROCEDURE [dbo].[Earthwatcher_GetFreeLand]
@BasecampDetailId int,
@TutorLandId int,
@GeoHexKey varchar(max)
AS
BEGIN
	select top 1 
		l.Id, 
		bld.Distance as distance1, 
		l.GeohexKey, 
		CASE WHEN l.Id IN (Select Land From EarthwatcherLands) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS usedLand
	from BasecampLandDistance bld
	inner join Land l on bld.LandId = l.Id
	where bld.BasecampDetailId = @BasecampDetailId
	and (l.Id NOT IN (Select Land From EarthwatcherLands) OR (l.Id IN (Select Land From EarthwatcherLands) AND l.LandStatus = 2 AND l.StatusChangedDateTime < DATEADD(hour, -1, GETUTCDATE())))
	and l.LandStatus > 0
	and l.Landthreat > 0 
	and l.IsLocked = 0
	and l.DemandAuthorities = 0
	and l.Id <> @TutorLandId --Parcela del tutor no se puede reshuffle
	and l.GeohexKey NOT IN (@GeoHexKey)
	order by l.BasecampId desc, l.Landthreat desc, distance1 asc
	                        
END
