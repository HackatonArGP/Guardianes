
CREATE PROCEDURE [dbo].[Lands_UpdateDemand] 
	@lands LandsType READONLY,
	@EarthwatcherId int
AS
BEGIN
	SET NOCOUNT ON;

    /* 1. Seteo DemandAuthorities */
    Update Land
    Set Confirmed = l.Confirmed, 
		ConfirmedBy = @EarthwatcherId,
		DemandAuthorities = CASE WHEN land.LandStatus = 4 AND l.Confirmed = 1 THEN 1 WHEN land.LandStatus = 3 AND l.Confirmed = 0 THEN 1 ELSE 0 END		
    From Land land
    Inner Join @lands l on l.GeohexKey = land.GeohexKey
    
    /* 2. Borro el historial de las confirmaciones verdes o rechazos amarillos */
    DECLARE @deleteHistory TABLE (Land INT)
    INSERT INTO @deleteHistory (Land)
    Select Id From Land 
    Where (LandStatus = 3 AND GeohexKey in (Select GeohexKey From @lands Where Confirmed = 1))
		OR (LandStatus = 4 AND GeohexKey in	(Select GeohexKey From @lands Where Confirmed = 0)) 
	
	Update Comments Set IsDeleted = 1 Where LandId in (Select Land From @deleteHistory)
	Update Verifications Set IsDeleted = 1 Where Land in (Select Land From @deleteHistory)
	
	/* 3. Si rechazo marco los amarillos como verdes y viceversa */
	Update Land
    Set Confirmed = l.Confirmed, LandStatus = CASE WHEN land.LandStatus = 3 THEN 4 WHEN land.LandStatus = 4 THEN 3 END
    From Land land
    Inner Join @lands l on l.GeohexKey = land.GeohexKey AND l.Confirmed = 0
    
    /* 4. Inserto las confirmaciones y deconfirmaciones de Greenpeace */    
    Insert Into Verifications (Land, Earthwatcher, IsAlert, AddedDate)
    Select land.Id, (Select Id From Earthwatcher Where Name = 'greenpeace@greenpeace.org'), 0, GETUTCDATE()
    From Land land
    Inner Join @lands l on l.GeohexKey = land.GeohexKey AND land.LandStatus = 3
    
    Insert Into Verifications (Land, Earthwatcher, IsAlert, AddedDate)
    Select land.Id, (Select Id From Earthwatcher Where Name = 'greenpeace@greenpeace.org'), 1, GETUTCDATE()
    From Land land
    Inner Join @lands l on l.GeohexKey = land.GeohexKey AND land.LandStatus = 4
    
    /* 5. Agrego las observaciones como comentarios */
    Insert Into Comments (EarthwatcherId, LandId, UserComment, Published)
    Select (Select Id From Earthwatcher Where Name = 'greenpeace@greenpeace.org'), land.Id, l.Observations, GETUTCDATE()
    From Land land
    Inner Join @lands l on l.GeohexKey = land.GeohexKey
    Where l.Observations IS NOT NULL AND l.Observations != ''
    
    --Insert Points
    Insert Into scores(EarthwatcherId, action, published, points, LandId)
    Select e.Id, 'DemandAuthoritiesApproved', GETUTCDATE(), 200, l.Id From @lands land 
    Inner Join land l on land.GeoHexKey = l.GeoHexKey
    Inner Join EarthwatcherLands el on l.Id = el.Land
    Inner Join Earthwatcher e on el.Earthwatcher = e.Id
    Left Join scores s on e.Id = s.EarthwatcherId and s.action = 'DemandAuthoritiesApproved'
    where l.DemandAuthorities = 1 and s.Id is null
    
END


