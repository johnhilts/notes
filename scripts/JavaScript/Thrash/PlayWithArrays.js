function ProcessArray(result)
{
    if(!result || result.toString().length==0 || result.toString().indexOf("|", 0) <= 0)
    {
        WScript.Echo("Unable to determine Status");
        return;
    }
    
    resultsArray = result.toString().split("|");
    if(resultsArray[0] == '1')
    {
        WScript.Echo("Happy!");
        WScript.Echo(resultsArray[1]);
    }
    else
    {
        WScript.Echo("Sad!");
        WScript.Echo(resultsArray[1]);
    }
}    

ProcessArray("0|Unavailable - find yo'self anothuh suckah!");

ProcessArray("1|Available - you are my friend!");

WScript.Echo("Blank");
ProcessArray("");

WScript.Echo("No Pipe");
ProcessArray("Aint no status hya!");

WScript.Echo("NULL");
ProcessArray(null);
