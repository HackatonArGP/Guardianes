CREATE PROCEDURE [dbo].[StatisticQuery_UnicLogin]
	@StartDate Datetime,
	@EndDate Datetime
AS
	select DATEADD(dd, 0, DATEDIFF(dd, 0, published)) as Fecha,	count(distinct EarthwatcherId) as Cantidad_de_Logins_Unicos_Por_Dia
	   from scores
			where action = 'login'  
			and published between @StartDate and @EndDate 
			group by (DATEADD(dd, 0, DATEDIFF(dd, 0, published))) 
			order by Fecha
