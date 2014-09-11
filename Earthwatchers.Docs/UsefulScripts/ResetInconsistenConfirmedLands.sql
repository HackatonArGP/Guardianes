

update l
set Confirmed = null,
	DemandAuthorities = 0
from Earthwatchers2.dbo.Land l
inner join EarthwatchersFeb.dbo.Land l2 on l.Id = l2.Id
where l2.Confirmed is null
  and l2.LandStatus = 4
  and l.Confirmed = 0
  and l.LandStatus = 4
  
  
  
  
select *  
from Earthwatchers2.dbo.Land l
inner join EarthwatchersFeb.dbo.Land l2 on l.Id = l2.Id
where l2.Confirmed is null
  and l2.LandStatus = 4
  and l.Confirmed = 0
  and l.LandStatus = 4
  