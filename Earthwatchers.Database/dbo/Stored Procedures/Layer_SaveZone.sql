CREATE PROCEDURE [dbo].[Layer_SaveZone] 
@name VARCHAR(50),
@description VARCHAR(200),
@layerId INT,
@param1 VARCHAR(10),
@ID INT output
AS
BEGIN
insert into Zones(Name, Description, LayerId, Param1) values(@name,@description,@layerId,@param1)SET @ID = SCOPE_IDENTITY()
END
