USE HealthLabsDirect
GO

declare @dtLastAltered SMALLDATETIME

SET @dtLastAltered = '10/17/06'

SELECT *
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE LAST_ALTERED >= @dtLastAltered
ORDER BY LAST_ALTERED DESC

SELECT *
FROM sys.views 
where modify_date >= @dtLastAltered
ORDER BY modify_date DESC

SELECT *
FROM sys.tables
where modify_date >= @dtLastAltered
ORDER BY modify_date DESC

/*
SELECT *
FROM INFORMATION_SCHEMA.VIEWS

SELECT *
FROM INFORMATION_SCHEMA.TABLES
*/
