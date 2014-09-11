CREATE PROCEDURE [dbo].[Comments_GetCommentsByUserId] 
@Id INT
AS
BEGIN
select a.Id as Id,a.EarthwatcherId as EarthwatcherId, a.LandId as LandId,a.UserComment as UserComment,a.Published as Published, l.Name as UserName, SUBSTRING(l.Name, 0, CHARINDEX('@', l.Name)) as FullName from Comments a left join Earthwatcher l on a.EarthwatcherId=l.Id where a.EarthwatcherId=@Id and a.IsDeleted = 0 order by a.Published Desc
END
