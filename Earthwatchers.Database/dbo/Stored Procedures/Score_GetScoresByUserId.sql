CREATE PROCEDURE [dbo].[Score_GetScoresByUserId] 
@Id INT
AS
BEGIN
select Id, EarthwatcherId, action, published, points, LandId, Param1, Param2 from Scores where earthwatcherid=@Id Order By published desc
END
