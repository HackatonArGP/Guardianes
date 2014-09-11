
CREATE PROCEDURE ReCalculate_LandDistanceFromBasecamps
AS
BEGIN
insert into BasecampLandDistance
select bc.Id, l.Id, l.Centroid.STDistance(bc.Location)
from Land l, BasecampDetails bc
where l.LandStatus > 0
and l.Landthreat > 0 
and l.IsLocked = 0
and l.DemandAuthorities = 0
and l.Id <> 64207
END