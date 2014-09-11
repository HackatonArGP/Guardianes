CREATE PROCEDURE [dbo].[Score_PostScore] 
@earthwatcherid INT, 
@action VARCHAR(255),
@points INT,
@landId INT = null,
@param1 NVARCHAR(50) = null,
@param2 NVARCHAR(50) = null,
@ID int output
AS
BEGIN
insert into scores(EarthwatcherId, action, published, points, LandId, Param1, Param2)
                values (@earthwatcherid, @action,GETUTCDATE(),@points, @landId, @param1, @param2)set @ID = SCOPE_IDENTITY()
END
