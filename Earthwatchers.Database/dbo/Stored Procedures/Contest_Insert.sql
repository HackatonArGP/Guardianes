CREATE PROCEDURE [dbo].[Contest_Insert] 
@startDate smalldatetime,
@endDate smalldatetime, 
@shortTitle varchar(50),
@title varchar(100),
@description varchar(500),
@imageURL varchar(100),
@ID INT output
AS
BEGIN
INSERT INTO Contest(StartDate, EndDate, ShortTitle, Title, Description, ImageURL) values(@startDate, @endDate, @shortTitle, @title, @description, @imageURL)SET @ID = SCOPE_IDENTITY()
END

