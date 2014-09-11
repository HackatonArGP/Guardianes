CREATE PROCEDURE [dbo].[Comments_DeleteComment] 
@commentId INT
AS
BEGIN
Update Comments Set IsDeleted = 1 where Id=@commentId
END
