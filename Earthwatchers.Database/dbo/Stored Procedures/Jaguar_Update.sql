CREATE PROCEDURE [dbo].[Jaguar_Update] 
@earthWatcherId INT,
@posId INT
AS
BEGIN
update JaguarPositions set FoundBy = (Select Name From Earthwatcher where Id = @earthWatcherId) where Id = @posId
END
