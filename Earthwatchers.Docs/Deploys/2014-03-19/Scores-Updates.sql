Update Scores
Set Param1 = SUBSTRING(Action, LEN('ContestWinnerAnnounced') + 1, LEN(Action))
	, Action = 'ContestWinnerAnnounced'
Where Action Like 'ContestWinnerAnnounced%'

Update Scores
Set Param1 = SUBSTRING(Action, LEN('ContestWon') + 1, LEN(Action))
	, Action = 'ContestWon'
Where Action Like 'ContestWon%'

Update Scores
Set Param1 = SUBSTRING(Action, LEN('DailyMessage') + 1, LEN(Action))
	, Action = 'DailyMessage'
Where Action Like 'DailyMessage%'

Update Scores
Set LandId = l.Id
	, Action = 'ConfirmationAdded'
From Scores s
Inner Join Land l on SUBSTRING(s.Action, LEN('ConfirmationAdded') + 2, LEN(s.Action)) = l.GeoHexKey
Where s.Action Like 'ConfirmationAdded%'

Update Scores
Set LandId = l.Id
	, Action = 'LandStatusChanged'
From Scores s
Inner Join Land l on SUBSTRING(s.Action, LEN('LandStatusChanged') + 2, LEN(s.Action)) = l.GeoHexKey
Where s.Action Like 'LandStatusChanged%'

Update Scores
Set LandId = l.Id
	, Action = 'LandReassigned'
From Scores s
Inner Join Land l on SUBSTRING(s.Action, LEN('LandReassigned') + 2, LEN(s.Action)) = l.GeoHexKey
Where s.Action Like 'LandReassigned%'



