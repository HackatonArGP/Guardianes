CREATE PROCEDURE [dbo].[Earthwatcher_HasSigned]
	@earthwatcher varchar(50),
	@petitionId int
AS
Begin
	SELECT Signed from PetitionSigned 
	where PetitionNumber = @petitionId and Earthwatcher = @earthwatcher
End