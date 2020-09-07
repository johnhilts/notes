# Add-Type -AssemblyName System.Xml.Ling

[Reflection.Assembly]::LoadWithPartialName("System.Xml.Linq") | Out-Null


[System.Xml.Linq.XElement]::Load("c:\work\lampsplus\source\sites\lampsplus\web.base.config")
