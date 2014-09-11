CREATE PROCEDURE [dbo].[Score_UpdateScore] 
@points INT,
@earthwatcherid INT,
@action VARCHAR(255)
AS
BEGIN
update scores set points = @points, published = GETUTCDATE() where EarthwatcherId = @earthwatcherid and action = @action
END
