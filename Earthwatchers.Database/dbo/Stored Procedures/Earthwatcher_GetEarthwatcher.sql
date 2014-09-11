CREATE PROCEDURE [dbo].[Earthwatcher_GetEarthwatcher] 
@Id INT
AS
BEGIN
select Id, EarthwatcherGuid as Guid, Name, Country, Role, IsPowerUser, Language, Region, NotifyMe, NickName, ApiEwId from Earthwatcher Where Id = @Id
END
