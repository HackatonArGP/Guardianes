CREATE PROCEDURE [dbo].[Earthwatcher_UpdatePassword] 
@prefix VARCHAR(255),
@hash VARCHAR(255),
@name VARCHAR(255)
AS
BEGIN
update Earthwatcher set PasswordPrefix = @prefix, HashedPassword = @hash where name = @name
END
