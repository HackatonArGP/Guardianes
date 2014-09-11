CREATE PROCEDURE [dbo].[Land_AddVerification] 
@LandId INT,
@isAlert BIT,
@Id INT
AS
BEGIN
IF NOT EXISTS (select Earthwatcher from Verifications where land = @LandId and earthwatcher = @Id and IsAlert = @isAlert and IsDeleted = 0)
	                                    BEGIN
		                                    Delete From Verifications where land = @LandId and earthwatcher = @Id
		                                    INSERT INTO Verifications (Land, Earthwatcher, IsAlert)
		                                    VALUES (@LandId, @Id, @isAlert)
		                                    Select count(Land) From Verifications Where land = @LandId
	                                    END
                                    ELSE
	                                    SELECT 0
END
