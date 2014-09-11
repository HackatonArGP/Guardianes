CREATE PROCEDURE [dbo].[Collections_GetCollectionItemsByEarthwatcher] 
@Id INT
AS
BEGIN
Select i.Id, c.Id CollectionId, c.Name CollectionName, i.Name, i.Icon, case when ec.CollectionItemId is not null then CAST(1 AS BIT) else CAST(0 AS BIT) end as HasItem, c.BackgroundImage From Collections c
                Inner Join CollectionItems i on c.Id = i.CollectionId
                Left Join EarthwatcherCollections ec on i.Id = ec.CollectionItemId and ec.EarthwatcherId = @Id
                order by c.Id
END
