CREATE PROCEDURE [dbo].[Score_AddLoginScore] 
@name VARCHAR(255)
AS
BEGIN
DECLARE @id INT 
                    SET @id = (Select Id From Earthwatcher Where Name = @name)
    
                     Insert Into scores(EarthwatcherId, action, published, points)
                    Select @id, 'Login', GETUTCDATE(), CASE WHEN (
			                    Select CASE WHEN COUNT(published) = 0 THEN 1 WHEN DATEDIFF(hour, MAX(published), GETUTCDATE()) >= 2 THEN 1 ELSE 0 END AS addlogin
			                    From (
				                    Select TOP 1 published From scores where action = 'Login' and EarthwatcherId = @id and points > 0 Order By published DESC) lastlogins
		                    ) = 1 THEN 100 ELSE 0 END
                     From Earthwatcher
					 Where Id = @id
END
