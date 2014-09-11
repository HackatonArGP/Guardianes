DECLARE @hexagon geography 
SET @hexagon = 'POLYGON((-64.2525863647461 -24.2184066772461,-64.2525863647461 -24.3725185394287,-63.9229965209961 -24.3725185394287,-63.9229965209961 -24.2184066772461,-64.2525863647461 -24.2184066772461))'

DECLARE @poly2 geography
SET @poly2 = 'POLYGON((-64.0 -24.2184066772461,-64.0 -24.3725185394287,-63.9229965209961 -24.3725185394287,-63.9229965209961 -24.2184066772461,-64.0 -24.2184066772461))'


Select location.STArea() as Area from
(
    select @poly1.STIntersection(@poly2) location
) data


--Select @poly1.STIntersection(@poly2)


'POLYGON((-63.7799339294434 -23.295467376709,-63.7738151550293 -23.295467376709,-63.7707748413086 -23.3003330230713,-63.7738227844238 -23.3051815032959,-63.779914855957 -23.3051605224609)
,-63.7829627990723 -23.3003330230713))'



Select * From Land where Id = 89871

DECLARE @longIncrease1 float
DECLARE @longIncrease2 float
DECLARE @latIncrease float

DECLARE @latExample float
DECLARE @longExample float

SET @longIncrease1 = 0.0075
SET @longIncrease2 = 0.003815789
SET @latIncrease = 0.005

SET @longExample = -63.777
SET @latExample = -23.3005

DECLARE @hexagon geometry 
SET @hexagon = 'POLYGON((' 
				+ CAST(@longExample - @longIncrease1 AS varchar(32)) + ' ' 
				+ CAST(@latExample AS varchar(32)) + ','
				+ CAST(@longExample - @longIncrease2 AS varchar(32)) + ' ' 
				+ CAST(@latExample + @latIncrease AS varchar(32)) + ','
				+ CAST(@longExample + @longIncrease2 AS varchar(32)) + ' ' 
				+ CAST(@latExample + @latIncrease AS varchar(32)) + ','
				+ CAST(@longExample + @longIncrease1 AS varchar(32)) + ' ' 
				+ CAST(@latExample AS varchar(32)) + ','
				+ CAST(@longExample + @longIncrease2 AS varchar(32)) + ' ' 
				+ CAST(@latExample - @latIncrease AS varchar(32)) + ','
				+ CAST(@longExample - @longIncrease2 AS varchar(32)) + ' ' 
				+ CAST(@latExample - @latIncrease AS varchar(32)) + ','
				+ CAST(@longExample - @longIncrease1 AS varchar(32)) + ' ' 
				+ CAST(@latExample AS varchar(32))
				+ '))'
				
Select location.STArea() as Area from
(
    Select PolygonGeom.MakeValid().STIntersection(@hexagon.STUnion(@hexagon.STStartPoint())) location From Polygons p
    Inner Join Zones z on p.ZoneId = z.Id
    Inner Join
    (
		
    )
    Where z.Name = ('Zona Roja')
) data


--Area = 0.000113000001397915




Select 
	* 
From Land where Id = 89871 
