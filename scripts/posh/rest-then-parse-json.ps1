$Uri = 'https://bamboo.lampsplus.com:8443/rest/api/latest/result/TES-REL11/?max-result=1'

$headers = @{
    Authorization = "Basic SECRET"
    Accept = "application/json"
}

$Result = Invoke-RestMethod -Uri $Uri -Method Get -headers $headers

$latestResultBaseUri = $Result.results.result[0].link.href
$artifact = "/?expand=artifacts"
$latestResultUri = "$latestResultBaseUri$artifact"
 # | Write-Output

Write-Host "URI: $latestResultUri"

$buildResult = Invoke-RestMethod -Uri $latestResultUri -Method Get -headers $headers

$buildResult.artifacts[0].artifact[0].name
$buildResult.artifacts[0].artifact[0].link.href
$buildResult.artifacts[0].artifact[0]
$buildResult.buildResultKey
 