CREATE PROCEDURE [dbo].[Land_GetVerifiedLandsGeoHexCodes_2] 
@earthwatcherId INT
AS
BEGIN
Select GeohexKey From Land Where (IsLocked = 1 OR DemandAuthorities = 1) AND Id Not In (Select Land From PollResults Where Earthwatcher = @earthwatcherId)
END
