CREATE PROCEDURE [dbo].[PopUpMessage_GetAllMessages] 
AS
BEGIN
select Id, ShortTitle, Title, Description, ImageURL,  StartDate , EndDate from PopupMessages
END

