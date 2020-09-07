$Url="https://localhost:44300/api/asset/count/?isConnected=false"


Invoke-RestMethod -Method 'Get' -Uri $Url # -Credential $Cred # -OutFile output.csv