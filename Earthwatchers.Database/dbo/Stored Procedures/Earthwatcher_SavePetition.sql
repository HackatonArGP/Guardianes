CREATE PROCEDURE [dbo].[Earthwatcher_SavePetition]
	@earthwatcher varchar(50),
	@petitionId int,
	@signed bit
AS
Begin
	insert into PetitionSigned (Earthwatcher,PetitionNumber,Signed) values (@earthwatcher, @petitionId, @signed)
End
