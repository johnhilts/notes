Use HealthLabsDirect
GO

DBCC SHRINKFILE(dbYourHealthIsVital_Log, 0)
GO

--BACKUP LOG HealthLabsDirect WITH TRUNCATE_ONLY
BACKUP LOG HealthLabsDirect TO DISK='NUL'
GO

DBCC SHRINKFILE(dbYourHealthIsVital_Log, 0)
GO

