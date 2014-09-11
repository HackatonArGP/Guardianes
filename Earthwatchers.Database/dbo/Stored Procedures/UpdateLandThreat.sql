CREATE PROCEDURE [dbo].[UpdateLandThreat]
AS
	UPDATE [dbo].[Land]
	SET [LandThreat] = T.[grid_code]
	FROM [dbo].[Land] L, [dbo].[geohexkeycodes] T
	WHERE T.[geohexkey]= L.[GeohexKey]

RETURN 0
