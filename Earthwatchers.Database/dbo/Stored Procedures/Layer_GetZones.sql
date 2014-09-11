CREATE PROCEDURE [dbo].[Layer_GetZones] 
@layerId INT
AS
BEGIN
select * from Zones where LayerId = @layerId
END
