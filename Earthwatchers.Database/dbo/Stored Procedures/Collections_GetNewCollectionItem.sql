CREATE PROCEDURE [dbo].[Collections_GetNewCollectionItem] 
@earthwatcherId INT
AS
BEGIN
DECLARE @id INT
                SET @id = (Select TOP 1 ci.Id From CollectionItems ci
                Left Join EarthwatcherCollections ec on ci.Id = ec.CollectionItemId
                Where ec.CollectionItemId is null
                ORDER BY NEWID())

                INSERT INTO EarthwatcherCollections (EarthwatcherId, CollectionItemId)
                VALUES (@earthwatcherId, (Select @id))

                Select i.Id, c.Id CollectionId, c.Name CollectionName, i.Name, i.Icon, CAST(1 AS BIT) HasItem, c.BackgroundImage From EarthwatcherCollections ec
                Inner Join CollectionItems i on ec.CollectionItemId = i.Id
                Inner Join Collections c on i.CollectionId = c.Id
                Where ec.EarthwatcherId = @earthwatcherId and ec.CollectionItemId = @id
END
