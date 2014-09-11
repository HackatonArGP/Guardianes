CREATE PROCEDURE [dbo].[Land_GetLandsToConfirm] 

AS
BEGIN
DECLARE @firstowners TABLE (Land INT, Earthwatcher INT)
                INSERT INTO @firstowners (Land, Earthwatcher)
                Select c.Land, c.Earthwatcher From Verifications c Inner Join (
	                Select Land, MIN(AddedDate) AddedDate From Verifications v
	                Inner Join Land l on v.Land = l.Id
	                Where (l.Confirmed is not null OR (l.Confirmed is null and Earthwatcher NOT IN (Select Id From Earthwatcher Where Name = 'greenpeace@greenpeace.org')))
	                Group By Land) fo on c.Land = fo.Land and c.AddedDate = fo.AddedDate

                Select l.Id, ROUND(l.Latitude,4) as Latitude, ROUND(l.Longitude,4) as Longitude, GeohexKey, Distance, LandThreat, LandStatus, CONVERT(varchar(10), lastc.AddedDate, 20) 'StatusChangedDateTime', DemandAuthorities, CONVERT(varchar(10), LastReset, 20) LastReset, e.Id as EarthwatcherId, e.Name LastUsersWithActivity, l.IsLocked
                    , l.Confirmed
                    , polln.Number Negatives
                    , pollp.Number Positives
                    , pollnv.Number NegativesV
                    , pollpv.Number PositivesV
                    , oks.Number OKs
                    , suspi.Number Suspicious
                 From Land l 
                 CROSS APPLY (
                         SELECT  TOP 1 Land,  Earthwatcher
                         FROM    @firstowners
                         WHERE   Land = l.Id
                         ) firstowner
                inner join (
	                Select Land, MAX(AddedDate) AddedDate From Verifications Group By Land 
                ) lastc on l.id = lastc.land
                 inner join Earthwatcher e on firstowner.Earthwatcher = e.Id
                 Left Join Verifications gpv on l.Id = gpv.Land and gpv.Earthwatcher IN (Select Id From Earthwatcher Where Name = 'greenpeace@greenpeace.org')
                 Left Join (Select Land, COUNT(Land) Number From PollResults r Inner Join Earthwatcher e on r.Earthwatcher = e.Id and e.Role <> 1 Where HasDeforestation = 1 Group By Land) polln on l.Id = polln.Land
                 Left Join (Select Land, COUNT(Land) Number From PollResults r Inner Join Earthwatcher e on r.Earthwatcher = e.Id and e.Role <> 1 Where HasDeforestation = 0 Group By Land) pollp on l.Id = pollp.Land
                 Left Join (Select Land, COUNT(Land) Number From PollResults r Inner Join Earthwatcher e on r.Earthwatcher = e.Id and e.Role = 1 Where HasDeforestation = 1 Group By Land) pollnv on l.Id = pollnv.Land
                 Left Join (Select Land, COUNT(Land) Number From PollResults r Inner Join Earthwatcher e on r.Earthwatcher = e.Id and e.Role = 1 Where HasDeforestation = 0 Group By Land) pollpv on l.Id = pollpv.Land
                 Left Join (Select Land, COUNT(Land) Number From Verifications Where IsAlert = 0 Group By Land) oks on l.Id = oks.Land
                 Left Join (Select Land, COUNT(Land) Number From Verifications Where IsAlert = 1 Group By Land) suspi on l.Id = suspi.Land
                Where l.LandStatus in (3,4) and l.IsLocked = 1
                order by pollnv.Number DESC, polln.Number DESC, lastc.AddedDate
END

