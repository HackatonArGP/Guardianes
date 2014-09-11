CREATE PROCEDURE [dbo].[SatelliteImage_Insert] 
@name VARCHAR(255),
@provider VARCHAR(255),
@published DATETIME2(7),
@acquisitiondate DATETIME2,
@extent VARCHAR(255),
@imagetype INT,
@urltilecache VARCHAR(255),
@minlevel INT,
@maxlevel INT,
@urlmetadata VARCHAR(255),
@iscloudy BIT,
@ID INT output
AS
BEGIN
insert into SatelliteImage(extent,Name,Provider,Published,AcquisitionDate,ImageType, UrlTileCache, MinLevel, MaxLevel, UrlMetadata, IsCloudy) values(GEOGRAPHY::STGeomFromText(@extent,4326),@name,@provider,@published,@acquisitiondate,@imagetype,@urltilecache,@minlevel,@maxlevel,@urlmetadata,@iscloudy)SET @ID = SCOPE_IDENTITY()
END
