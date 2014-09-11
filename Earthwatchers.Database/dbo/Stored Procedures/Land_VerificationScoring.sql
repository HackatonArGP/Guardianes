CREATE PROCEDURE [dbo].[Land_VerificationScoring] 
@LandId INT,
@GreenpeaceId INT
AS
BEGIN
Select TOP 1 Earthwatcher From Verifications Where Land = @LandId and Earthwatcher != @GreenpeaceId Order By AddedDate
END
