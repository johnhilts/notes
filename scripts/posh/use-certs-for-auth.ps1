$cert = $null
# Get-ChildItem Cert:\LocalMachine\My | ForEach-Object -Process {$friendly = $_.FriendlyName;if ($friendly "Wildcard_lampsplus.com") Write-Host "Friendly name: $friendly"}
foreach ($certItem in Get-ChildItem Cert:\LocalMachine\My)
{
    $friendly = $certItem.FriendlyName
    if ($friendly -eq "Wildcard_lampsplus.com") 
    {
        # Write-Host "Friendly name: $friendly"
        $cert = $certItem
    }
}

if ($cert -ne $null)
{
    $cert.FriendlyName
}

$cert.Thumbprint

$Url="https://bamboo.lampsplus.com:8443/rest/api/latest/result/TES-REL11/?max-result=1"
Invoke-RestMethod -Method 'Get' -Uri $Url -CertificateThumbprint $cert.Thumbprint
