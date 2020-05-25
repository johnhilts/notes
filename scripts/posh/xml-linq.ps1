# get reference to LINQ to XML
[Reflection.Assembly]::LoadWithPartialName("System.Xml.Linq") | Out-Null

# read and parse the xml in test results file
$testResultsXmlFileName = `
# "\\lpvmdata\WebDevData\Data\JohnHilts\WorkNotes\AutomatedTesting\Archtecture\Troubleshooting\AllBatches.20191114-Morning.xml"
"C:\work\test-automation\LampsPlus.RegressionTests\bin\ReleaseGrid\Results\20191411 132446  Regression Results.xml"
$testResultsXml = [System.Xml.Linq.XElement]::Parse([System.Xml.Linq.XElement]::Load($testResultsXmlFileName))

# locate tests
$tests = $testResultsXml.Descendants("test")

# locate just tests with failures
$failures = $tests | Where-Object {$PSItem.Descendants("failure") | Select-Object -First 1}

$methods = `
    $failures `
    | Select-Xml -XPath "//test" `
    | Select-Object -ExpandProperty Node `
    | Select-Object -Property type,method `
    | foreach {$PSItem.type + "." + $PSItem.method} `
    | Sort-Object -Unique

$limit = 1000
$newList  = New-Object System.Collections.Generic.List[string]
$sum = 0
$methodParameters = ""
$testRunner = "`"c:\work\test-automation\packages\xunit.runner.console.2.4.1\tools\net472\xunit.console.exe`" LampsPlus.RegressionTests.dll"
$xmlLogInfo = "-xml TestResults\RerunResults.xml"

foreach ($method in $methods)
{
  $methodParameters += " -method $method"
  $sum += $method.Length
  if ($sum -ge $limit)
  {
    $newList.Add("$testRunner $methodParameters $xmlLogInfo")
    $sum = 0
    $methodParameters = ""
  }
}
if ($methodParameters.Length -gt 0)
{
    $newList.Add("$testRunner $methodParameters $xmlLogInfo")
}
$newList
$methodCount = $methods.Count
Write-Host "There are $methodCount failed methods."
$newList.Add("exit 0")
$output = $newList -join "`n"
$output
$output | Out-File "test.txt" -Encoding ascii

