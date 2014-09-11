CREATE PROCEDURE [dbo].[UpdateDistanceLand]
AS
	DECLARE @startpoint geography;
	SET @startpoint =geography::STGeomFromText('POINT(112.6964666666667 -0.1440333333333334)', 4326);
		
	UPDATE [dbo].[Land]
	SET [Distance] = [Centroid].STDistance(@startpoint)
RETURN
