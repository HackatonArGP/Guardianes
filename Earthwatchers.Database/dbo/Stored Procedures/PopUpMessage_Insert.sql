CREATE PROCEDURE [dbo].[PopUpMessage_Insert] 
@startDate smalldatetime,
@endDate smalldatetime, 
@shortTitle varchar(50),
@title varchar(100),
@description varchar(MAX),
@imageURL varchar(100),
@ID INT output
AS
BEGIN
INSERT INTO PopupMessages(StartDate, EndDate, ShortTitle, Title, Description, ImageURL) values(@startDate, @endDate, @shortTitle, @title, @description, @imageURL)SET @ID = SCOPE_IDENTITY()
END
