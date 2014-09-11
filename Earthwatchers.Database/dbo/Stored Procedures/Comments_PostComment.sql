CREATE PROCEDURE [dbo].[Comments_PostComment] 
@userid INT,
@landid INT,
@Comment VARCHAR(255),
@Published DATETIME2(7),
@ID INT output
AS
BEGIN
insert into Comments(EarthwatcherId, LandId, UserComment, Published) values(@userid,@landid,@Comment,@Published) SET @ID = SCOPE_IDENTITY()
END
