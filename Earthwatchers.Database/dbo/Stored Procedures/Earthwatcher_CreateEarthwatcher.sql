CREATE PROCEDURE [dbo].[Earthwatcher_CreateEarthwatcher] 
@guid UNIQUEIDENTIFIER,
@name VARCHAR(255),
@role INT,
@prefix VARCHAR(255),
@hash VARCHAR(255),
@country VARCHAR(255),
@language VARCHAR(255),
@nick VARCHAR(255) = null,
@ID INT output
AS
BEGIN
insert into Earthwatcher(EarthwatcherGuid, Name, Role, PasswordPrefix, HashedPassword, country, language, NickName) values(@guid,@name,@role,@prefix,@hash,@country,@language,@nick)set @ID = SCOPE_IDENTITY()
END
