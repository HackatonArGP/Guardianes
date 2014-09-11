CREATE PROCEDURE [dbo].[Land_AddPoll] 
@GeohexKey VARCHAR(11),
@EarthwatcherId INT,
@IsUsed BIT
AS
BEGIN
Insert Into PollResults (Land, Earthwatcher, HasDeforestation)
                VALUES ((Select Id From Land Where GeohexKey = @GeohexKey), @EarthwatcherId, @IsUsed)
END
