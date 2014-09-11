CREATE PROCEDURE [dbo].[Jaguar_Insert] 
@day INT,
@hour INT, 
@minutes INT,
@longitude FLOAT(50),
@latitude FLOAT(50),
@ID INT output
AS
BEGIN
INSERT INTO JaguarPositions (Day, Hour, Minutes, Point) values(@day, @hour, @minutes, geography::STPointFromText('POINT(' + CAST(@longitude AS VARCHAR(20)) + ' ' + CAST(@latitude AS VARCHAR(20)) + ')', 4326))SET @ID = SCOPE_IDENTITY()
END
