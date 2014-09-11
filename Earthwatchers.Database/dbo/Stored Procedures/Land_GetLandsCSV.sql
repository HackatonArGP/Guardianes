CREATE PROCEDURE [dbo].[Land_GetLandsCSV] 

AS
BEGIN
select l.Id, l.Latitude, l.Longitude, GeohexKey, LandThreat, LandStatus, StatusChangedDateTime = CONVERT(varchar(10), StatusChangedDateTime, 20), DemandAuthorities, LastReset = CONVERT(varchar(10), LastReset, 20), e.Id as EarthwatcherId, ISNULL(e.NickName, e.Name) as  EarthwatcherName
                , OKsDetail = (SELECT STUFF((
		                SELECT ( ';' + ISNULL(e.NickName, e.Name))
		                FROM Verifications c
		                INNER JOIN Earthwatcher e on c.Earthwatcher = e.Id
		                WHERE l.Id = c.Land and c.IsAlert = 0
		                ORDER BY 
		                  AddedDate DESC
		                FOR XML PATH( '' )),1,1, ''))
                , SuspiciousDetail = (SELECT STUFF((
		                SELECT ( ';' + ISNULL(e.NickName, e.Name))
		                FROM Verifications c
		                INNER JOIN Earthwatcher e on c.Earthwatcher = e.Id
		                WHERE l.Id = c.Land and c.IsAlert = 1
		                ORDER BY 
		                  AddedDate DESC
		                FOR XML PATH( '' )),1,1, ''))
                , OKs = (Select COUNT(Land) From Verifications c Where IsAlert = 0 AND l.Id = c.Land)
                , Suspicious = (Select COUNT(Land) From Verifications c Where IsAlert = 0 AND l.Id = c.Land)
                , null Negatives
                , null Positives
                , null NegativesV
                , null PositivesV
                from Earthwatcher e
                left join EarthwatcherLands el on el.Earthwatcher = e.Id
                left join Land l on l.Id = el.Land
                order by e.Id
END
