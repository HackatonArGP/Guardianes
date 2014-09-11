
/*

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

Truncate Table TempLandIntersection

DBCC SHRINKDATABASE(Earthwatchers)
*/

USE [Earthwatchers]
GO
/****** Object:  StoredProcedure [dbo].[AssignLandToEarthwatcher]    Script Date: 01/30/2014 20:23:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertTempLandIntersection]
	@from INT,
	@to INT
AS
BEGIN
	SET NOCOUNT ON;
	
DECLARE @longIncrease1 float
DECLARE @longIncrease2 float
DECLARE @latIncrease float

DECLARE @latExample float
DECLARE @longExample float

SET @longIncrease1 = 0.0060939789855
SET @longIncrease2 = 0.0030460358703
SET @latIncrease = 0.004795071177501

SET @longExample = -63.777
SET @latExample = -23.3005
/*
Select
				hexagon = CAST('POLYGON((' 
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
				+ '))' AS geometry).STArea() 
				
				/*


--Inserto en una tabla temporal las lands con su hexagono
Insert Into dbo.TempLandIntersection (Land, Hexagon, PolygonGeom, LandThreat)
Select
	l.Id
			, hexagon = CAST('POLYGON((' 
				+ CAST(Longitude - @longIncrease1 AS varchar(32)) + ' ' 
				+ CAST(Latitude AS varchar(32)) + ','
				+ CAST(Longitude - @longIncrease2 AS varchar(32)) + ' ' 
				+ CAST(Latitude + @latIncrease AS varchar(32)) + ','
				+ CAST(Longitude + @longIncrease2 AS varchar(32)) + ' ' 
				+ CAST(Latitude + @latIncrease AS varchar(32)) + ','
				+ CAST(Longitude + @longIncrease1 AS varchar(32)) + ' ' 
				+ CAST(Latitude AS varchar(32)) + ','
				+ CAST(Longitude + @longIncrease2 AS varchar(32)) + ' ' 
				+ CAST(Latitude - @latIncrease AS varchar(32)) + ','
				+ CAST(Longitude - @longIncrease2 AS varchar(32)) + ' ' 
				+ CAST(Latitude - @latIncrease AS varchar(32)) + ','
				+ CAST(Longitude - @longIncrease1 AS varchar(32)) + ' ' 
				+ CAST(Latitude AS varchar(32))
				+ '))' AS geometry)
			, p.PolygonGeom
			, CASE When z.Name = 'Zona Roja' THEN 5 ELSE 3 END
		From Land l
		Inner Join Polygons p on 1 = 1 
		Inner Join Zones z on p.ZoneId = z.Id and z.Name in ('Zona Roja', 'Zona Amarilla')
		Where l.Id Between 101 AND 100

/*	
Update TempLandIntersection 
Set AreaIntersection = null
*/

--Calculo el area de la interseccion entre el hexagono y los poligonos rojos y amarillos
Update TempLandIntersection 
Set AreaIntersection = land.PolygonGeom.STIntersection(land.Hexagon).STArea()  
From TempLandIntersection land
Where land.AreaIntersection is null 

/*
Update TempLandIntersection 
Set LandThreat = CASE When z.Name = 'Zona Roja' THEN 5 ELSE 3 END
From TempLandIntersection land
Inner Join Polygons p on CAST(land.PolygonGeom AS VARCHAR(MAX)) = CAST(p.PolygonGeom AS VARCHAR(MAX))
Inner Join Zones z on p.ZoneId = z.Id and z.Name in ('Zona Roja', 'Zona Amarilla')
Where land.AreaIntersection > 0

Select TOP 1 CAST(PolygonGeom AS VARCHAR(MAX))  From TempLandIntersection
*/
END

--Select distinct Land From TempLandIntersection Where LandThreat = 5 and AreaIntersection >= 0.000078624000134714
Select distinct Land From TempLandIntersection Where LandThreat = 3 and AreaIntersection > 0 and AreaIntersection < 0.000078624000134714

Select MAX(Land) From TempLandIntersection

Select Land From TempLandIntersection Group By Land Having Sum(AreaIntersection) >=  0.000078624000134714

/*
USE [Earthwatchers2]
GO

/****** Object:  Table [dbo].[TempLandIntersection]    Script Date: 01/29/2014 16:42:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TempLandIntersection](
	[Land] [int] NOT NULL,
	[Hexagon] [geometry] NOT NULL,
	[AreaIntersection] [float] NULL,
	[PolygonGeom] [geometry] NOT NULL
) ON [PRIMARY]

GO



*/