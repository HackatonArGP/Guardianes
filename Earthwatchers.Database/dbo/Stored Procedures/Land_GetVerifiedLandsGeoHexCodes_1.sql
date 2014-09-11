CREATE PROCEDURE [dbo].[Land_GetVerifiedLandsGeoHexCodes_1] 

AS
BEGIN
Select GeohexKey From Land Where (IsLocked = 1 OR DemandAuthorities = 1)
END
