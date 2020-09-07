#Param(
#	[parameter(Mandatory=$true)]
#	[string]$ProxyUrl
#)

$ProxyPort = (Get-ChildItem env:DesignatedProxyPort).Value
$ProxyUri = "http://10.30.75.32:8081/proxy/$ProxyPort/hosts"
$headers = @{
    ContentType = "application/json"
}

$Body = "{""www.lampsplus.com"":""172.24.105.20""}"
$Body
$contentType = 'application/json'

$ProxyDnsSetResult = Invoke-RestMethod -Uri $ProxyUri -Method Post -headers $headers -Body $Body -ContentType $contentType
$ProxyDnsSetResult
