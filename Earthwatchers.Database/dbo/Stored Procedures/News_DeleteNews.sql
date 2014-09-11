CREATE PROCEDURE [dbo].[News_DeleteNews] 
@Id INT
AS
BEGIN
delete from News where Id=@Id
END
