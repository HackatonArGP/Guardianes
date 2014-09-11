/*
Update Land
Set LandStatus = 1, earthwatcherguid = null, demandauthorities = 0, demandurl = null
Where earthwatcherguid in (
Select earthwatcherguid From earthwatcher where id > 1)
*/

delete from Verifications

delete from comments
DBCC CHECKIDENT('comments', RESEED, 0)

delete from scores
DBCC CHECKIDENT('scores', RESEED, 0)

delete from EarthwatcherLands
delete from EarthwatcherCollections
delete from PollResults
delete from JaguarPositions
DBCC CHECKIDENT('JaguarPositions', RESEED, 0)

delete from earthwatcher where name not in ('Tutor@greenpeace.org', 'lgorganchian@gmail.com', 'greenpeace@greenpeace.org')

Update Land
Set LandStatus = 1, StatusChangedDateTime = null, IsLocked = 0, Confirmed = 0, DemandAuthorities = 0
Where LandThreat > 0 and LandStatus > 1

INSERT INTO EarthwatcherLands (Land, Earthwatcher)
VALUES (106953, (Select TOP 1 Id From Earthwatcher where Name = 'Tutor@greenpeace.org'))

Update Land
Set LandStatus = 3, StatusChangedDateTime = GETUTCDATE()
Where Id = 106953

/*

Delete From Locations
DBCC CHECKIDENT ('Locations', RESEED, 0)

Delete From Polygons
DBCC CHECKIDENT ('Polygons', RESEED, 0)

Delete From Zones
DBCC CHECKIDENT ('Zones', RESEED, 0)

Delete From Layers
DBCC CHECKIDENT ('Layers', RESEED, 0)

*/








