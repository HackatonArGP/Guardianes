CREATE PROCEDURE [dbo].[Flag_DeleteFlag] 
@Id INT
AS
BEGIN
delete from Flags where Id=@Id
END
