CREATE PROCEDURE [dbo].[Land_VerificationScoring_2] 
@LandId INT,
@GreenpeaceId INT
AS
BEGIN
Select TOP 1 e.Name From Verifications v Inner Join Earthwatcher e on v.Earthwatcher = e.Id Where v.Land = @LandId and v.Earthwatcher != @GreenpeaceId Order By v.AddedDate
END
