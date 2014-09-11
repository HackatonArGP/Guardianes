CREATE PROCEDURE [dbo].[Basecamp_Edit] 
@basecampid INT,
@longitude FLOAT(50),
@latitude FLOAT(50),
@probability INT,
@name VARCHAR(200),
@shortText VARCHAR(MAX),
@id INT
AS
BEGIN

UPDATE BasecampDetails set BasecampId = @basecampId, Location = geography::STPointFromText('POINT(' + CAST(@longitude AS VARCHAR(20)) + ' ' + CAST(@latitude AS VARCHAR(20)) + ')', 4326), Probability = @probability, Name = @name, ShortText = @shortText where Id = @id

END
