CREATE PROCEDURE [dbo].[Earthwatcher_GetAllEarthwatchers] 

AS
BEGIN
select Id, EarthwatcherGuid as Guid, Name, Role, IsPowerUser, Language, Region, NotifyMe, NickName from Earthwatcher
END
