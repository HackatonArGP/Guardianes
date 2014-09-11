CREATE PROCEDURE [dbo].[Earthwatcher_GetApiEw] 
@api VARCHAR(50),
@userId VARCHAR(255)
AS
BEGIN
select * from ApiEwLogin where Api = @api and UserId = @userId
END