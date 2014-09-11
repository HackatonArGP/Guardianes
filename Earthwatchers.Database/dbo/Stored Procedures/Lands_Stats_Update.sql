
CREATE PROCEDURE [dbo].[Lands_Stats_Update] 
	
AS
BEGIN
	SET NOCOUNT ON;

	Truncate Table LandStats
		
	INSERT INTO LandStats (ShowOrder, Name, Number, Percentage, UOM)
 
    Select ShowOrder = 1, Name = 'StatsActivePlayers', Number = count(Earthwatcher), Percentage = CAST(0 AS SMALLMONEY), UOM = '' From (Select distinct Earthwatcher From Verifications Where AddedDate >= DATEADD(day, -5, getutcdate())) data
	 UNION
	 Select 1, 'StatsTotalPlayers', count(Id), CAST(0 AS SMALLMONEY), '' From Earthwatcher
	 UNION
	 Select 2, 'StatsGreenPlots', count(Id), CAST(0 AS SMALLMONEY), '' From Land Where LandStatus = 3
	 UNION
	 Select 3, 'StatsRedPlots', count(Id), CAST(0 AS SMALLMONEY), '' From Land Where LandStatus = 4
	 UNION
	 Select 2, 'StatsVerifiedPlots', count(Id), CAST(0 AS SMALLMONEY), '' From Land Where DemandAuthorities = 1
	 UNION
	 Select 3, 'StatsDenouncesCreated', count(EarthwatcherId), CAST(0 AS SMALLMONEY), '' From (Select EarthwatcherId, action From scores where action Like 'DemandAuthorities%') data
	 UNION
	 
	 --Multiplicador de 0.7 dependerá del tamaño del hexágono final
	 Select 4, 'StatsAlertedArea', (Select CAST(ROUND(count(Id) * 0.7,0) AS INT) From Land Where LandStatus = 4), 
		(Select (Select CAST(count(Id) AS SMALLMONEY) From Land Where LandStatus = 4) / (Select CAST(count(Id) AS SMALLMONEY) From Land Where LandThreat > 0)), 'km2'
	 UNION
	 
	 Select 4, 'StatsAlertedAreaConfirmed', (Select CAST(ROUND(count(Id) * 0.7,0) AS INT) From Land Where LandStatus = 4 and Confirmed = 1), 
		(Select (Select CAST(count(Id) AS SMALLMONEY) From Land Where LandStatus = 4 and Confirmed = 1) / (Select CAST(count(Id) AS SMALLMONEY) From Land Where LandThreat > 0)), 'km2'
	 
	 UNION
	 
	 Select 4, 'StatsOkAreaConfirmed', (Select CAST(ROUND(count(Id) * 0.7,0) AS INT) From Land Where LandStatus = 3 and Confirmed = 1), 
		(Select (Select CAST(count(Id) AS SMALLMONEY) From Land Where LandStatus = 3 and Confirmed = 1) / (Select CAST(count(Id) AS SMALLMONEY) From Land Where LandThreat > 0)), 'km2'
   
END

