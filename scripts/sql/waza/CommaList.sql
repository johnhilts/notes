DECLARE @stuff VARCHAR(8000)
SET @stuff = '' 
SELECT @stuff = sValue + ',' + @stuff
FROM tblNameValuePairs

select @stuff