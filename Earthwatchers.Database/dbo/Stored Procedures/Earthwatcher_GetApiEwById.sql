CREATE PROCEDURE [dbo].[Earthwatcher_GetApiEwById] 
@apiEwId int
AS
BEGIN
select * from ApiEwLogin where Id = @apiEwId
END