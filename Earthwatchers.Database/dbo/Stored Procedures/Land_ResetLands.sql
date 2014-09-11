CREATE PROCEDURE [dbo].[Land_ResetLands] 
@TutorLandId int
AS
BEGIN
Update verifications set IsDeleted = 1 where Land in (select Id from Land where LandStatus = 3) --Borro las verificaciones de las parcelas verdes
Update Comments set IsDeleted = 1 where LandId in (select Id from Land where LandStatus = 3) --Borro los comentarios de las parcelas verdes
Update Land set confirmed = null where LandStatus = 3 -- Borro lac confirmacion de greenpeace de que era verde
Update Land set IsLocked = 0 where LandStatus = 3 and Id != @TutorLandId -- Desbloqueo las parcelas verdes, para que se vuelvan a asignar

Update Land  --Reseteo el estado de las parcelas verdes a 1 "Sin Asignar"
Set LandStatus = CASE WHEN LandStatus = 3 THEN 1 
				 ELSE LandStatus END, LastReset = GETUTCDATE() 
				 Where LandStatus > 1 and Id != @TutorLandId
END

