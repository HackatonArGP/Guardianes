CREATE PROCEDURE [dbo].[Layer_GetLayerByName] 
@Name VARCHAR(50)
AS
BEGIN
select * from Layers where Name = @Name
END
