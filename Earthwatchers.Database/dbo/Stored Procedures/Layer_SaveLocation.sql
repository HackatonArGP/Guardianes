CREATE PROCEDURE [dbo].[Layer_SaveLocation] 
@latitude FLOAT(50),
@longitude FLOAT(50),
@ID INT output,
@index INT,
@polygonId INT
AS
BEGIN
insert into Locations(Latitude, Longitude, [Index], PolygonId) values(@latitude,@longitude,@index,@polygonId)SET @ID = SCOPE_IDENTITY()
END
