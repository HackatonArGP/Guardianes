CREATE PROCEDURE [dbo].[Earthwatcher_UpdateAccessToken]
	@ApiEwId int,
	@AccessToken varchar(max)
AS
BEGIN
	Update ApiEwLogin set AccessToken = @AccessToken where Id = @ApiEwId
END