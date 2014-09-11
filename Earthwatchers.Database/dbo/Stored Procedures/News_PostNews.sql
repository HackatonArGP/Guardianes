CREATE PROCEDURE [dbo].[News_PostNews] 
@shape GEOGRAPHY,
@userid INT,
@Published DATETIME2(7),
@NewsItem VARCHAR(MAX),
@ID INT output
AS
BEGIN
insert into News(Shape, EarthwatcherId, Published, NewsItem) values(@shape,@userid,@Published,@NewsItem)SET @ID = SCOPE_IDENTITY()
END
