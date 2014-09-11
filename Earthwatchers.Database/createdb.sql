DROP PROCEDURE [earthwatchers].[UpdateLandThreat]
GO

DROP PROCEDURE [earthwatchers].[GetSatelliteImagesInExtent]
GO

DROP PROCEDURE [earthwatchers].[AssignLandToEarthwatcher]
GO

DROP TABLE [earthwatchers].[DeforestationDeconfirmers]
GO

DROP TABLE [earthwatchers].[DeforestationConfirmers]
GO

DROP TABLE [earthwatchers].[Land]
GO

DROP TABLE [earthwatchers].[Earthwatcher]
GO

DROP TABLE [earthwatchers].[SatelliteImage]
GO

DROP TABLE [earthwatchers].[ThreatLevel]
GO

DROP TABLE [earthwatchers].[Comments]
GO


CREATE TABLE [earthwatchers].[ThreatLevel]
(
    Id  INT IDENTITY NOT NULL,
    Shape GEOGRAPHY NOT NULL,
    ThreatLevel INT NOT NULL
)
GO

ALTER TABLE [earthwatchers].[ThreatLevel]
	ADD CONSTRAINT [PrimaryKeyConstraintThreatLevel]
	PRIMARY KEY (Id)
GO

CREATE SPATIAL INDEX [ThreatLevelShape] 
    ON [earthwatchers].[ThreatLevel]
	(Shape)
GO

CREATE TABLE [earthwatchers].[SatelliteImage]
(
    Id  INT IDENTITY NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Provider NVARCHAR(255) NOT NULL,
    Extent GEOGRAPHY NOT NULL,
    Published DATETIME2,
    ImageType INT NOT NULL
)
GO

ALTER TABLE [earthwatchers].[SatelliteImage]
	ADD CONSTRAINT [PrimaryKeyConstraintSatelliteImage]
	PRIMARY KEY (Id)
GO

CREATE SPATIAL INDEX [SpatialIndexSatelliteImageExtent] 
    ON [earthwatchers].[SatelliteImage]
	(Extent)
GO

CREATE TABLE [earthwatchers].[Earthwatcher]
(
    Id  INT IDENTITY NOT NULL,
    EarthwatcherGuid UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(255) NOT NULL
)
GO

ALTER TABLE [earthwatchers].[Earthwatcher]
	ADD CONSTRAINT [PrimaryKeyConstraintEarthwatcher]
	PRIMARY KEY (Id)
GO

CREATE UNIQUE INDEX [EarthwatcherGuidUnique]
    ON [earthwatchers].[Earthwatcher]
	(EarthwatcherGuid)
GO

CREATE UNIQUE INDEX [EarthwatcherNameUnique]
    ON [earthwatchers].[Earthwatcher]
	(Name)
GO

CREATE TABLE [earthwatchers].[Land]
(
    Id  INT IDENTITY NOT NULL,
    Centroid GEOGRAPHY NOT NULL,
    GeohexKey NVARCHAR(11) NOT NULL,
    LandType INT NOT NULL,
    EarthwatcherGuid UNIQUEIDENTIFIER,
    LandThreat INT DEFAULT 0 NOT NULL,
    LandStatus INT DEFAULT 1 NOT NULL,
    Distance FLOAT
)
GO

ALTER TABLE [earthwatchers].[Land]
	ADD CONSTRAINT [PrimaryKeyConstraintLand]
	PRIMARY KEY (Id)
GO

ALTER TABLE [earthwatchers].[Land]
	ADD CONSTRAINT [ForeignKeyConstraintLandEarthWatcher] 
	FOREIGN KEY (EarthwatcherGuid)
	REFERENCES [earthwatchers].[Earthwatcher] (EarthwatcherGuid)
GO

CREATE SPATIAL INDEX [SpatialIndexLandCentroid] 
    ON [earthwatchers].[Land]
	(Centroid)
GO

CREATE TABLE [earthwatchers].[DeforestationConfirmers]
(
    Land INT NOT NULL,
    Earthwatcher INT NOT NULL
)
GO

CREATE TABLE [earthwatchers].[Settings]
(
    Name NVARCHAR(255) NOT NULL,
    Val NVARCHAR(255) NOT NULL
)

ALTER TABLE [earthwatchers].[Settings]
	ADD CONSTRAINT [PrimaryKeyConstraintSettings]
	PRIMARY KEY (Name)
GO


CREATE TABLE [earthwatchers].[Comments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EarthwatcherId] [int] NOT NULL,
	[LandId] [int] NOT NULL,
	[UserComment] [nvarchar](255) NOT NULL,
	[Published] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO


ALTER TABLE [earthwatchers].[DeforestationConfirmers]
	ADD CONSTRAINT [PrimaryKeyConstraintDeforestationConfirmers]
	PRIMARY KEY (Land, Earthwatcher)
GO

ALTER TABLE [earthwatchers].[DeforestationConfirmers]
	ADD CONSTRAINT [ForeignKeyConstraintDeforestationConfirmersEarthwatcher] 
	FOREIGN KEY (Earthwatcher)
	REFERENCES [earthwatchers].[Earthwatcher] (Id)
GO

ALTER TABLE [earthwatchers].[DeforestationConfirmers]
	ADD CONSTRAINT [ForeignKeyConstraintDeforestationConfirmersLand] 
	FOREIGN KEY (Land)
	REFERENCES [earthwatchers].[Land] (Id)
GO

CREATE TABLE [earthwatchers].[DeforestationDeconfirmers]
(
    Land INT NOT NULL,
    Earthwatcher INT NOT NULL
)
GO

ALTER TABLE [earthwatchers].[DeforestationDeconfirmers]
	ADD CONSTRAINT [ForeignKeyConstraintDeforestationDeconfirmersEarthwatcher]
	FOREIGN KEY (Earthwatcher)
	REFERENCES [earthwatchers].[Earthwatcher] (Id)
GO

ALTER TABLE [earthwatchers].[DeforestationDeconfirmers]
	ADD CONSTRAINT [ForeignKeyConstraintDeforestationDeconfirmersLand] 
	FOREIGN KEY (Land)
	REFERENCES [earthwatchers].[Land] (Id)
GO

ALTER TABLE [earthwatchers].[DeforestationDeconfirmers]
	ADD CONSTRAINT [PrimaryKeyConstraintDeforestationDeconfirmers]
	PRIMARY KEY (Land, Earthwatcher)
GO

-- stored procedures

CREATE PROCEDURE [earthwatchers].[AssignLandToEarthwatcher]
	@EarthwatcherGuid UNIQUEIDENTIFIER,
	@GeohexKey NVARCHAR(11) OUTPUT
AS
    SET @GeohexKey =(SELECT TOP 1 GeohexKey 
                        FROM [Earthwatchers].[Land]
                        WHERE [EarthwatcherGuid] IS NULL
                        AND [Landthreat] > 0
                        AND [LandStatus] > 0
                        ORDER BY [Landthreat] DESC, [Distance] ASC)
    IF @GeohexKey IS NULL RETURN -1

    UPDATE [Earthwatchers].[Land]
        SET EarthwatcherGuid = @EarthwatcherGuid
        WHERE [GeohexKey] = @GeohexKey;

RETURN 0
GO

CREATE PROCEDURE [earthwatchers].[GetSatelliteImagesInExtent]
	@GeometryString NVARCHAR(MAX)
AS

    DECLARE  @QueryGeometry GEOGRAPHY
    
    SET @QueryGeometry = GEOGRAPHY::STGeomFromText(@GeometryString, 4326);

    SELECT Id, Name, Provider, Extent, Published, ImageType
    FROM [SatelliteImage] 
    WHERE @QueryGeometry.STIntersects([Extent]) = 1

RETURN
GO

CREATE PROCEDURE [earthwatchers].[UpdateLandThreat]
AS
	UPDATE [earthwatchers].[Land]
	SET [LandThreat] = T.[grid_code]
	FROM [earthwatchers].[Land] L, [dbo].[geohexkeycodes] T
	WHERE T.[geohexkey]= L.[GeohexKey]

RETURN 0
GO

CREATE PROCEDURE [earthwatchers].[UpdateDistanceLand]
AS
	DECLARE @startpoint geography;
	SET @startpoint =geography::STGeomFromText('POINT(111.492991 0.0808295)', 4326);
		
	UPDATE [earthwatchers].[Land]
	SET [Distance] = [Centroid].STDistance(@startpoint)
RETURN
GO

-- grant db rights
GRANT CONTROL ON SCHEMA :: earthwatchers TO Editor
GO



CREATE INDEX [IDXCommentEarthwatcherId] 
    ON [earthwatchers].[Comments]
	(EarthwatcherId)
GO

CREATE INDEX [IDXCommentLandId] 
    ON [earthwatchers].[Comments]
	(LandId)
GO

CREATE INDEX [IDXEarthwatcherName] 
    ON [earthwatchers].[Earthwatcher]
	(Name)
GO




-- insert data
INSERT INTO [earthwatchers].[ThreatLevel] (ThreatLevel, Shape)
	SELECT grid_code, geom 
	FROM [dbo].[ThreadLevel]
GO


--- Changes BT 7-7-2011
--- add column 'LandStatus' to Land
ALTER TABLE Land
ADD LandStatus INT DEFAULT 1 NOT NULL


--- Changes BT 19-7-2011
--- add columns Url to satelliteimage
ALTER TABLE SatelliteImage
ADD UrlTileCache NVARCHAR(255) NOT NULL

ALTER TABLE SatelliteImage
ADD UrlRawData NVARCHAR(255)

ALTER TABLE SatelliteImage
Add TilingStarted DATETIME2

ALTER TABLE SatelliteImage
Add TilingFinished DATETIME2

-- sample data for satelliteimages 
insert into SatelliteImage (Extent, Name, Provider, Published, ImageType)
values (geography::STGeomFromText('POLYGON((0 0, 115 0, 110 20, 0 20, 0 0))', 4326), 'testimage1', 'bertsens','07/12/2011', 1)

-- backup threatlevel data
select Cast(Id as varchar) + ','+ shape.STAsText() + ',' + Cast(ThreatLevel as varchar) from earthwatchers.ThreatLevel
-- backup earthwatchers

-- backup land

-- backup complete database
CREATE DATABASE BACKUPEARTHWATCHERS AS COPY OF Earthwatchers


update satelliteimage set Name='Landsat Testimage' where id=4
update satelliteimage set extent=geography::STGeomFromText('POLYGON((110.45623613620323 -0.94757626073443,112.67274733119231 -0.94757626073443,112.67274733119231 0.94182448835432,110.45623613620323 0.94182448835432, 110.45623613620323 -0.94757626073443))', 4326) where id=4
update satelliteimage set UrlTileCache='http://geodan.blob.core.windows.net/landsat/test/LE71200602008218EDC00' where Id=4
update satelliteimage set TilingStarted='07/20/2011' where Id=4
update satelliteimage set TilingFinished='07/21/2011' where Id=4
update satelliteimage set UrlRawData='' where Id=4
update satelliteimage set Provider='Nasa' where Id=4
update satelliteimage set ImageType=3 where Id=4

-- 22 juli 2011
-- update landthreat assignment:
-- use new shape ThreatLevel01Wgs instead of the stored procedure
update earthwatchers.land
set landthreat=0
from earthwatchers.Land l

update earthwatchers.land
set landthreat=t.grid_code
from earthwatchers.Land l, dbo.ThreatLevel01Wgs t
where l.Centroid.STIntersects(t.geom)=1 

-- 4 augustus 2011
-- adding rapideye truecolor
insert into SatelliteImage (Extent, Name, Provider, Published, ImageType, UrlTileCache)
values (geography::STGeomFromText('POLYGON((111.42689356084753 0.64689593060339, 111.65165102415294 0.64689593060339, 111.65165102415294 1.09016951575048, 111.42689356084753 1.09016951575048, 111.42689356084753 0.64689593060339))', 4326), 
'RapidEye TrueColor', 
'RapidEye', 
'07/08/2011',
3,
'http://geodan.blob.core.windows.net/satellite/2011-07-08_RE4_mosaic_RGB')


-- adding rapideye cir
insert into SatelliteImage (Extent, Name, Provider, Published, ImageType, UrlTileCache)
values (geography::STGeomFromText('POLYGON((111.42689356084753 0.64689593060339, 111.65165102415294 0.64689593060339, 111.65165102415294 1.09016951575048, 111.42689356084753 1.09016951575048, 111.42689356084753 0.64689593060339))', 4326), 
'RapidEye CIR', 
'RapidEye', 
'07/08/2011',
4,
'http://geodan.blob.core.windows.net/satellite/2011-07-08_RE4_mosaic_CIR')

-- 24 augustus
-- adding brazil images
insert into SatelliteImage (Extent, Name, Provider, Published, ImageType, UrlTileCache)
values (geography::STGeomFromText('POLYGON ((-43.09236359908854 -22.49757139197785,-42.85471458057571 -22.49757139197785,-42.85471458057571 -22.16959605782121, -43.09236359908854 -22.16959605782121, -43.09236359908854 -22.49757139197785))', 4326), 
'Teresopolis Brazil', 
'NASA', 
'02/02/2011',
3,
'http://geodan.blob.core.windows.net/satellite/teresopolis_ali_2011033_crop_geo')

-- 15 sept
-- adding intermap sar images of borneo
insert into SatelliteImage (Extent, Name, Provider, Published, ImageType, UrlTileCache)
values (geography::STGeomFromText('POLYGON((111.484240 -0.024885, 111.824792 -0.024885, 111.824792 0.264010, 111.484240 0.264010, 111.484240 -0.024885))', 4326), 
'Intermap SAR', 
'Intermap', 
'06/21/2007',
4,
'http://geodan.blob.core.windows.net/satellite/20070621_intermap_sar')

-- 20/9/2011 add a role for each earthwatcher, default=0
ALTER TABLE Earthwatcher
ADD Role INT DEFAULT 0 NOT NULL

-- 20/9/2011 add news table
CREATE TABLE [earthwatchers].[News]
(
    Id  INT IDENTITY NOT NULL,
    Shape GEOGRAPHY NOT NULL,
	 EarthwatcherId int NOT NULL,
	 Published datetime2(7) NOT NULL,
	 NewsItem nvarchar(255) NOT NULL,
)
GO


==============================
20 may 2012
==============================
task: fill special area

select geometry::STGeomFromText('POLYGON ((111.685585848673 0.119692925228692, 111.700116620867 0.117963177398468, 111.708071136168 0.123151853138755, 111.709450499816 0.124534229883233, 111.720530904321 0.134417121008003, 111.731944140378 0.145639582976033, 111.737134471953 0.159478514922469, 111.722259619204 0.172972889038533, 111.702884558989 0.175739730957495, 111.667941852117 0.17089294642702, 111.648222722529 0.167433491663951, 111.628159965813 0.163279302315996, 111.6309197125 0.143913438591387, 111.685585848673
0.119692925228692))',4326).STCentroid().ToString()

select * from earthwatchers.land l where l.centroid.STWithin(geography::STGeomFromText('POLYGON ((111.685585848673 0.119692925228692, 111.700116620867 0.117963177398468, 111.708071136168 0.123151853138755, 111.709450499816 0.124534229883233, 111.720530904321 0.134417121008003, 111.731944140378 0.145639582976033, 111.737134471953 0.159478514922469, 111.722259619204 0.172972889038533, 111.702884558989 0.175739730957495, 111.667941852117 0.17089294642702, 111.648222722529 0.167433491663951, 111.628159965813 0.163279302315996, 111.6309197125 0.143913438591387, 111.685585848673
0.119692925228692))',4326))= 1
select * from earthwatchers.earthwatcher e where e.name='eduardodias'


update earthwatchers.land
set landthreat=0
from earthwatchers.Land l

update earthwatchers.land
set landthreat=1
from earthwatchers.Land l
where l.centroid.STWithin(geography::STGeomFromText('POLYGON ((111.685585848673 0.119692925228692, 111.700116620867 0.117963177398468, 111.708071136168 0.123151853138755, 111.709450499816 0.124534229883233, 111.720530904321 0.134417121008003, 111.731944140378 0.145639582976033, 111.737134471953 0.159478514922469, 111.722259619204 0.172972889038533, 111.702884558989 0.175739730957495, 111.667941852117 0.17089294642702, 111.648222722529 0.167433491663951, 111.628159965813 0.163279302315996, 111.6309197125 0.143913438591387, 111.685585848673
0.119692925228692))',4326))= 1 

DECLARE @poly geography;
set @poly=geography::STGeomFromText('POLYGON ((111.685585848673 0.119692925228692, 111.700116620867 0.117963177398468, 111.708071136168 0.123151853138755, 111.709450499816 0.124534229883233, 111.720530904321 0.134417121008003, 111.731944140378 0.145639582976033, 111.737134471953 0.159478514922469, 111.722259619204 0.172972889038533, 111.702884558989 0.175739730957495, 111.667941852117 0.17089294642702, 111.648222722529 0.167433491663951, 111.628159965813 0.163279302315996, 111.6309197125 0.143913438591387, 111.685585848673
0.119692925228692))',4326)

DECLARE @startpoint geography;
SET @startpoint =geography::STGeomFromText('POINT (111.68486576273506 0.14973283455836695)', 4326)

UPDATE earthwatchers.Land
SET Distance = Centroid.STDistance(@startpoint)
where Centroid.STWithin(@poly)= 1 

select l.Centroid.STDistance(@startpoint)
from earthwatchers.Land l
where l.Centroid.STWithin(@poly)= 1 


=====================================
2 juni 2012
=====================================
update earthwatchers.land
set landthreat=0
from earthwatchers.Land l

update earthwatchers.land
set landthreat=1
from earthwatchers.Land l
where l.centroid.STWithin(geography::STGeomFromText('POLYGON ((111.6952675887905 0.09858689815320937, 111.6833246386072 0.07983107225956164, 111.6764961308078 0.05509936388098943,
111.7096881332694 0.0573683141082359, 111.7325699361353 0.06149124471585526, 111.7389592882494 0.0839603432977116,
111.7265909451294 0.09983431994614943, 111.7020750699571 0.105789611430313, 111.6952675887905 0.09858689815320937))',4326)
)= 1 

DECLARE @poly geography;
set @poly=geography::STGeomFromText('POLYGON ((111.6952675887905 0.09858689815320937, 111.6833246386072 0.07983107225956164, 111.6764961308078 0.05509936388098943,
111.7096881332694 0.0573683141082359, 111.7325699361353 0.06149124471585526, 111.7389592882494 0.0839603432977116,
111.7265909451294 0.09983431994614943, 111.7020750699571 0.105789611430313, 111.6952675887905 0.09858689815320937))',4326)

DECLARE @startpoint geography;
SET @startpoint =geography::STGeomFromText('POINT (111.7064179957322 0.07908191707662259)', 4326)

UPDATE earthwatchers.Land
SET Distance = Centroid.STDistance(@startpoint)
where Centroid.STWithin(@poly)= 1 

=====================================
28 juni 2012
=====================================
query to check land that can be assigned:
earthwatchers.Land
select * from land where LandThreat=1 and EarthwatcherGuid=null


-- back to old land
update earthwatchers.land
set landthreat=0
from earthwatchers.Land l

update earthwatchers.land
set landthreat=t.grid_code
from earthwatchers.Land l, dbo.ThreatLevel01Wgs t
where l.Centroid.STIntersects(t.geom)=1 

-- backup database
CREATE DATABASE BACKUPEARTHWATCHERS_120628 AS COPY OF Earthwatchers

=====================================
10 juli 2012
=====================================
' adding basecamp table

CREATE TABLE [earthwatchers].[Basecamp]
(
    Id  INT IDENTITY NOT NULL,
    Shape GEOGRAPHY NOT NULL,
    Name NVARCHAR(255) NOT NULL
)
GO

ALTER TABLE [earthwatchers].[Basecamp]
	ADD CONSTRAINT [PrimaryKeyConstraintBasecamp]
	PRIMARY KEY (Id)
GO

' add one record in borneo
DECLARE @sintang geography;
SET @sintang =geography::STGeomFromText('POINT (111.7064179957322 0.07908191707662259)', 4326);
insert into earthwatchers.Basecamp (Shape, Name) values (@sintang, 'Sintang');

' add another record in amazon
DECLARE @greenpeace geography;
SET @greenpeace =geography::STGeomFromText('POINT (-62 -25)', 4326);
insert into earthwatchers.Basecamp (Shape, Name) values (@greenpeace, 'Greenpeace');

' todo create land in argentinia

' set landthreat in argentinia in area
update earthwatchers.land
set landthreat=1
from earthwatchers.Land l
where l.centroid.STWithin(select geography::STGeomFromText('POLYGON ((-63 -26,-61 -26, -61 -24, -63 -24, -63 -26))',4326))= 1

=====================================
12 juli 2012
=====================================
' update basecamp location argentina
DECLARE @argentina geography;
SET @argentina =geography::STGeomFromText('POINT (-62.0140173201813 -25.22815257110068)', 4326);
update earthwatchers.Basecamp set Shape = @argentina where name='argentina'

' set threat area in argentina
' area: POLYGON ((-63.429565 -25.646479, -62.341919 -24.382124, -62.347412 -24.096619, -61.759644 -24.342093, -61.430054 -24.632038, -61.221313 -24.637032, -61.089478 -24.841581, -60.556641 -25.185059, -60.210571 -25.527573, -60.183105 -25.646479, -60.012817 -25.700937, -59.661255 -26.032106, -59.661255 -26.130781, -61.710205 -26.111053, -61.715698 -25.651430, -63.429565 -25.646479))


bounding: 
longitude: -64 -59
latitude: -24 -27

DECLARE @argentina geography;
SET @argentina =geography::STGeomFromText('PO (-62 -25)', 4326);
    

delete from earthwatchers.land where Centroid.Long < 0

 
=====================================
16 juli 2012
=====================================
' select land from argentina:
q: select count(*) from earthwatchers.land where Centroid.Long < 0
a: 19503

q: select land within threat area
select count(*) from earthwatchers.land l
where l.centroid.STWithin(geography::STGeomFromText('POLYGON ((-61.71792059886302 -26.13847026130058, -60.61198382557924 -25.94389088111966, -60.17244891350991 -25.65001833037828,
-60.19392386450425 -25.49220784377901, -62.32452048762647 -24.0916900214295, -62.91431844774216 -24.15974402685749,
-62.88743903255001 -24.52370647619604, -62.68331635897056 -24.68993403345444, -63.47116533207539 -25.57538759583879,
-63.38372113260081 -25.82757088546559, -61.71792059886302 -26.13847026130058))',4326))= 1
a: 5011

q: update landthreat in threat area in argentina
a:
update earthwatchers.land
set landthreat=1
where centroid.STWithin(geography::STGeomFromText('POLYGON ((-61.71792059886302 -26.13847026130058, -60.61198382557924 -25.94389088111966, -60.17244891350991 -25.65001833037828,
-60.19392386450425 -25.49220784377901, -62.32452048762647 -24.0916900214295, -62.91431844774216 -24.15974402685749,
-62.88743903255001 -24.52370647619604, -62.68331635897056 -24.68993403345444, -63.47116533207539 -25.57538759583879,
-63.38372113260081 -25.82757088546559, -61.71792059886302 -26.13847026130058))',4326))= 1

q: select user in argentina
select * from earthwatchers.land where Centroid.Long < 0  and earthwatcherguid is not null
a: land.id=59025

=========================================
19 juli reassign land eduardo for demo
=========================================

select * from earthwatchers.earthwatcher where name='eduardodias'
'guid: 5F498A6B-C379-447C-81F6-CA71FE89B91E
select * from earthwatchers.land where earthwatcherguid='5F498A6B-C379-447C-81F6-CA71FE89B91E'
' land id: 17177

select * from earthwatchers.earthwatcher where name='konopkova'
' guid: 661FC22E-BF64-4360-8175-73E13CBF529F

select * from earthwatchers.land where earthwatcherguid='661FC22E-BF64-4360-8175-73E13CBF529F'
' land id=16136

' land id below konopkova: 16137
select * from earthwatchers.land where id=16137

update EARTHWATCHERS.land set earthwatcherguid=null,landstatus=1 where id=17177
update earthwatchers.land set earthwatcherguid='5F498A6B-C379-447C-81F6-CA71FE89B91E', landstatus=2 where id=16137


=========================================
7 march 2013 add score table (BT)
=========================================
CREATE TABLE earthwatchers.scores (
Id [int] IDENTITY(1,1) NOT NULL,
EarthwatcherId [int] NOT NULL,
action [nvarchar](255) NOT NULL,
published [datetime2](7) NOT NULL,
points [int] NOT NULL)

create CLUSTERED index cdxId on earthwatchers.scores
(
	[Id] ASC
)

create index cdxEwId on earthwatchers.scores
(
	[Earthwatcherid] ASC
)

ALTER TABLE earthwatchers.scores
  ADD CONSTRAINT uniqueScores UNIQUE (EarthwatcherId,Action)

ALTER TABLE earthwatchers.Earthwatcher
ADD Telephone NVARCHAR(255)

 





