CREATE PROCEDURE [dbo].[Layer_SaveLayer] 
@name VARCHAR(50),
@description VARCHAR(200),
@ID INT output
AS
BEGIN
insert into Layers(Name, Description) values(@name,@description)SET @ID = SCOPE_IDENTITY()
END
