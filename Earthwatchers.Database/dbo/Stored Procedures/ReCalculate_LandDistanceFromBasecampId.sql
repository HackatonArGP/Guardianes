CREATE PROCEDURE [dbo].[ReCalculate_LandDistanceFromBasecampId]
@id int
AS
BEGIN

delete from BasecampLandDistance where BasecampDetailId = @id
insert into BasecampLandDistance
select bc.Id, l.Id, l.Centroid.STDistance(bc.Location)
from Land l, BasecampDetails bc
where l.LandStatus > 0
and l.Landthreat > 0 
and l.IsLocked = 0
and l.DemandAuthorities = 0
and l.Id <> 64207
and bc.Id = @id
END