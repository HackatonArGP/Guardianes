CREATE PROCEDURE [dbo].[Lands_ForceLandsReset] 
	@lands LandsType READONLY,
	@EarthwatcherId int
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @toReset TABLE (Land INT)
    INSERT INTO @toReset (Land)
    Select Id From Land 
    Where GeohexKey in (Select GeohexKey From @lands) 
		
	/*
	Borrado lógico de todas las verificaciones
	Borrado lógico de todos los comentarios
	*/
	Update Comments Set IsDeleted = 1 Where LandId in (Select Land From @toReset)
	Update Verifications Set IsDeleted = 1 Where Land in (Select Land From @toReset)
	
	--Desasignar la Land al usuario que la tuviese asignada (El usuario de Greenpeace en estos casos)
	Delete From EarthwatcherLands Where Land In (Select Land From @toReset)
	
	/*
	Cambiar el LandStatus a 1 (para que vuelva a estar asignable)
	Setear como false el DemandAuthorities
	Setear como No confirmada
	Setear como No Lockeada.
	*/
Update Land
	Set LandStatus = 1, DemandAuthorities = 0, Confirmed = null, IsLocked = 0, ReportResetted = GETUTCDATE()
	Where GeohexKey in (Select GeohexKey From @lands) 
    
END


