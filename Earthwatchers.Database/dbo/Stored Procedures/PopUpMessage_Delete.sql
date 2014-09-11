
CREATE PROCEDURE [dbo].[PopUpMessage_Delete] 
@id INT
AS
BEGIN
delete from PopupMessages where id=@id
END

