param([ValidateSet("x86","x64")][string]$OfficeArchitecture)
$ErrorActionPreference="Stop"
$Root=Split-Path -Parent $PSScriptRoot
$Msi=Join-Path $Root "release\Installers\WordProSuite.Desktop.$OfficeArchitecture.msi"
$Log=Join-Path $env:LOCALAPPDATA "WordProSuite\Logs\WordProSuite.log"
if(-not(Test-Path $Msi)){throw "MSI not found: $Msi"}
Get-Process WINWORD -ErrorAction SilentlyContinue|Stop-Process -Force
Start-Process msiexec.exe -ArgumentList "/i `"$Msi`" /qn" -Wait
$word=New-Object -ComObject Word.Application
$word.Visible=$false
$doc=$word.Documents.Add()
$addin=$word.COMAddIns.Item("WordProSuite.Desktop.AddIn")
$addin.Connect=$true
Start-Sleep 3
if(-not $addin.Connect){throw "Word did not connect the COM add-in"}
if(-not(Test-Path $Log)){throw "Startup log not found"}
if((Get-Content $Log -Raw)-notmatch "OnConnection"){throw "OnConnection not confirmed"}
$doc.Close(0);$word.Quit()
[Runtime.InteropServices.Marshal]::FinalReleaseComObject($doc)|Out-Null
[Runtime.InteropServices.Marshal]::FinalReleaseComObject($word)|Out-Null
Start-Process msiexec.exe -ArgumentList "/x `"$Msi`" /qn" -Wait
Write-Host "Office 2024 integration test passed"
