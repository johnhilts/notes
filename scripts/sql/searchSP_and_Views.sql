use MyYagooft
go

SELECT ROUTINE_NAME as 'Store proc' 
 FROM INFORMATION_SCHEMA.ROUTINES 
    WHERE ROUTINE_DEFINITION LIKE '%try%' 
    and ROUTINE_DEFINITION LIKE '%commit%' 
and ROUTINE_TYPE='procedure' -- 'function'

SELECT TABLE_NAME as 'View' 
 FROM INFORMATION_SCHEMA.VIEWS
    WHERE VIEW_DEFINITION LIKE '%tblEmail_AutoResponderText%'