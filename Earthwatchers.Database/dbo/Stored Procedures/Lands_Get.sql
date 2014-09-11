
--Lands_Get 0, 0, 0, '','', NULL, 0
--Lands_Get 17570
--Lands_Get 0, 1, 2
--Lands_Get 0, 0, 0, 'greenpeace@greenpeace.org'
--Lands_Get 0, 0, 0, '','NZ665144'
--Lands_Get 12795, 0, 0, '','', 'POLYGON((-64.2525863647461 -24.2184066772461,-64.2525863647461 -24.3725185394287,-63.9229965209961 -24.3725185394287,-63.9229965209961 -24.2184066772461,-64.2525863647461 -24.2184066772461))'
--Lands_Get 0, 0, 0, '','', NULL, 4
CREATE PROCEDURE [dbo].[Lands_Get] 
	@id INT = 0
	, @getAll BIT = 0
	, @earthwatcherId INT = 0
	, @name NVARCHAR(255) = ''
	, @geohexKey NVARCHAR(11) = ''
	, @wkt NVARCHAR(255) = 'POLYGON EMPTY'
	, @status INT = 0
AS
BEGIN
	SET NOCOUNT ON;

    select l.Id, l.Latitude, l.Longitude, bd.ShortText as ShortText, bd.Id as BasecampId, bd.Name as BasecampName, GeohexKey, Distance, LandThreat, LandStatus, StatusChangedDateTime, DemandAuthorities, DemandUrl, LastReset, el.Earthwatcher as EarthwatcherId, ISNULL(e.NickName, SUBSTRING(e.[Name], 0, CHARINDEX('@', e.Name))) as EarthwatcherName, e.IsPowerUser, l.IsLocked, NULL Confirmed, ISNULL(v.OKs, '') OKs, ISNULL(v.Alerts, '') Alerts, LTRIM(v.LastUsersWithActivity) LastUsersWithActivity
	from Land l 
	left join EarthwatcherLands el on l.Id = el.Land
	left join Earthwatcher e on el.Earthwatcher = e.Id 
	left join BasecampDetails bd on l.BasecampId = bd.Id
	left join (
		select
			Land
			, stuff((
				select ',' + CAST(t.[Earthwatcher] AS VARCHAR(10))
				from Verifications t
				where t.Land = Verifications.Land and IsAlert = 0 and IsDeleted = 0
				order by t.AddedDate
				for xml path('')
			),1,1,'') as OKs
			, stuff((
				select ',' + CAST(t.[Earthwatcher] AS VARCHAR(10))
				from Verifications t
				where t.Land = Verifications.Land and IsAlert = 1 and IsDeleted = 0
				order by t.AddedDate
				for xml path('')
			),1,1,'') as Alerts
			, stuff((
				select ', ' + ISNULL(e.NickName, SUBSTRING(e.[Name], 0, CHARINDEX('@', Name)))
				from Verifications t
				inner join Earthwatcher e on t.Earthwatcher = e.Id
				where t.Land = Verifications.Land and t.IsDeleted = 0
				order by t.AddedDate
				for xml path('')
			),1,1,'') as LastUsersWithActivity
		from Verifications
		group by Land
	) v on l.Id = v.Land
	where 
		((@id = 0 AND @wkt = 'POLYGON EMPTY') OR (@id != 0 AND @wkt = 'POLYGON EMPTY' AND l.Id = @id) OR (@id != 0 AND @wkt != 'POLYGON EMPTY'))
		AND (@name = '' OR (@name != '' AND e.Name = @name))
		AND (@geohexKey = '' OR (@geohexKey != '' AND l.GeohexKey = @geohexKey))
		AND (@getAll = 0 OR (@getAll = 1 AND (l.LandStatus > 2 OR (l.LandStatus = 2 AND el.Earthwatcher = @earthwatcherId))))  
		AND (@wkt = 'POLYGON EMPTY' OR (@wkt != 'POLYGON EMPTY' AND (l.LandStatus > 2 OR (l.LandStatus = 2 AND l.Id = @id)) AND centroid.STIntersects(geography::STGeomFromText(@wkt, 4326)) = 1))
		AND (@status = 0 OR (@status != 0 AND l.LandStatus = @status))
END


 