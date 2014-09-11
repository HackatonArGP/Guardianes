Select firstlogin, count(EarthwatcherId) From (
Select EarthwatcherId, CAST(MIN(published) AS DATE) firstlogin From scores group by EarthwatcherId) data
group by firstlogin
order by firstlogin

Select count(e.Id) From Earthwatcher e 
Left Join (
Select EarthwatcherId, CAST(MIN(published) AS DATE) firstlogin From scores group by EarthwatcherId) d on e.Id = d.EarthwatcherId
Where d.EarthwatcherId is null


Select published, count(EarthwatcherId) From (
Select EarthwatcherId, CAST(published AS DATE) published From scores where action like 'LandStatusChanged%' or action like 'ConfirmationAdded%'
group by EarthwatcherId, CAST(published AS DATE)) data
group by published
order by published

--Cuantos usuarios registrados en el dia chekearon una parcela ese mismo dia
Select firstlogin, count(EarthwatcherId) From (
Select r.EarthwatcherId, r.firstlogin From 
	(Select EarthwatcherId, CAST(MIN(published) AS DATE) firstlogin From scores group by EarthwatcherId) r
	Inner Join (Select EarthwatcherId, CAST(published AS DATE) published From scores where action like 'LandStatusChanged%' or action like 'ConfirmationAdded%'
group by EarthwatcherId, CAST(published AS DATE)) a on r.EarthwatcherId = a.EarthwatcherId and r.firstlogin = a.published) data
group by firstlogin
order by firstlogin

--Cuantos usuarios registrados chekeron una parcela el dia siguiente
Select firstlogin, count(EarthwatcherId) From (
Select r.EarthwatcherId, r.firstlogin From 
	(Select EarthwatcherId, CAST(MIN(published) AS DATE) firstlogin From scores group by EarthwatcherId) r
	Inner Join (Select EarthwatcherId, CAST(published AS DATE) published From scores where action like 'LandStatusChanged%' or action like 'ConfirmationAdded%'
group by EarthwatcherId, CAST(published AS DATE)) a on r.EarthwatcherId = a.EarthwatcherId and DATEADD(day, 1, r.firstlogin) = a.published) data
group by firstlogin
order by firstlogin

--Cuantos usuarios registrados chekeron una parcela a los 7 días
Select firstlogin, count(EarthwatcherId) From (
Select r.EarthwatcherId, r.firstlogin From 
	(Select EarthwatcherId, CAST(MIN(published) AS DATE) firstlogin From scores group by EarthwatcherId) r
	Inner Join (Select EarthwatcherId, CAST(published AS DATE) published From scores where action like 'LandStatusChanged%' or action like 'ConfirmationAdded%'
group by EarthwatcherId, CAST(published AS DATE)) a on r.EarthwatcherId = a.EarthwatcherId and DATEADD(day, 7, r.firstlogin) = a.published) data
group by firstlogin
order by firstlogin