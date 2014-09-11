CREATE PROCEDURE [dbo].[Flag_PostFlag] 
@userid INT,
@ID INT output,
@location GEOGRAPHY,
@comment VARCHAR(255),
@published DATETIME2(7)
AS
BEGIN
insert into Flags(EarthwatcherId, Location, Comment, Published) values(@userid, @location, @comment, @published)SET @ID = SCOPE_IDENTITY()
END
