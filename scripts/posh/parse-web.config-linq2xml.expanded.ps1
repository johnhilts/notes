# get reference to LINQ to XML
[Reflection.Assembly]::LoadWithPartialName("System.Xml.Linq") | Out-Null

# read and parse the xml in web.config
$configXml = [System.Xml.Linq.XElement]::Parse([System.Xml.Linq.XElement]::Load("c:\work\lampsplus\source\sites\lampsplus\web.config"))

# locate element
$elmahAttributes = $configXml.Descendants("elmah").Elements().Attributes() | where Name -EQ "applicationName" 

# update in memory
$elmahAttributes.Value = $elmahAttributes.Value.Replace("97", "101")
$elmahAttributes.Value 

# save to disk
# $configXml.Save("c:\work\lampsplus\source\sites\lampsplus\web.config")