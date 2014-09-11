--Despues de correr el importer de amarillos
Update TempLands
Set LandThreat = 3 Where LandThreat is null

--Despues de correr el importer de rojos
Update TempLands
Set LandThreat = 5 Where LandThreat is null


DECLARE @lands TABLE (Id INT)
INSERT INTO @lands (Id)
Select l.Id From Land l
Inner Join EarthwatcherLands el on l.Id = el.Land
Left Join TempLands tl on l.Id = tl.Land
Where l.DemandAuthorities = 0 and tl.Land is null and l.LandThreat > 0

Delete From EarthwatcherLands Where Land In (Select Id From @lands)

--Delete From Verifications Where Land In (Select Id From @lands)

Update Land
Set LandStatus = 1, LandThreat = 0, IsLocked = 0, Confirmed = null, LastReset = GETDATE()
Where Id In (Select Id From @lands)

Update Land
Set LandStatus = 1, LandThreat = 0, IsLocked = 0, Confirmed = null, LastReset = GETDATE()
Where Id In (Select l.Id From Land l
		Left Join TempLands tl on l.Id = tl.Land
		Where l.DemandAuthorities = 0 and tl.Land is null and l.LandThreat > 0)

Update Land
Set LandThreat = 3
From Land l 
Inner Join TempLands tl on l.Id = tl.Land and tl.LandThreat = 3

Update Land
Set LandThreat = 5
From Land l 
Inner Join TempLands tl on l.Id = tl.Land and tl.LandThreat = 5


Select Count(Id) From Land Where LandThreat > 0