CREATE PROCEDURE [dbo].[BackupDb]
AS

DBCC SHRINKDATABASE(Earthwatchers)

DECLARE @path varchar(200) = 'Z:\Web\Backups\Earthwatchers_' + CAST(CONVERT(date,GETDATE(),101) AS VARCHAR(10)) + '.bak'

BACKUP DATABASE Earthwatchers TO DISK = @path WITH INIT


