CREATE PROCEDURE [dbo].[Contest_GetAllContests] 

AS
BEGIN
select Id, ShortTitle, Title, Description, ImageURL,  StartDate , EndDate, WinnerId from Contest
END

/****** Object:  StoredProcedure [dbo].[Contest_Insert]    Script Date: 02/11/2014 19:24:15 ******/
SET ANSI_NULLS ON
