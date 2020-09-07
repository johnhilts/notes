$Uri = 'https://bamboo.lampsplus.com:8443/rest/api/latest/info'

$headers = @{
    Authorization = "Basic YWRtaW46YWRtaW4="
    Accept = "application/json"
}

$Result = Invoke-RestMethod -Uri $Uri -Method Get -headers $headers

$Result | Write-Output