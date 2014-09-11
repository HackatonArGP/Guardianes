
CREATE PROCEDURE [dbo].[Lands_Stats] 
	
AS
BEGIN
	SET NOCOUNT ON;
	
	Select ShowOrder, Name, Number, Percentage, UOM From LandStats 
	where Name = 'StatsTotalPlayers' or Name = 'StatsVerifiedPlots' or Name = 'StatsAlertedAreaConfirmed' or Name = 'StatsDenouncesCreated'

END

