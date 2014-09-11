CREATE PROCEDURE [dbo].[Earthwatcher_CreateApiEwLogin] 
@Api VARCHAR(50),
@UserId VARCHAR(255),
@NickName VARCHAR(255) = null,
@SecretToken VARCHAR(255) = null,
@AccessToken VARCHAR(MAX),
@Mail VARCHAR(255) = null,
@ID INT output
AS
BEGIN
insert into ApiEwLogin(Api, UserId, NickName, AccessToken, SecretToken, Mail) values(@Api, @UserId, @NickName, @AccessToken, @SecretToken, @Mail) set @ID = SCOPE_IDENTITY()
END