param([ValidateSet('x86','x64')][string]$OfficeArchitecture)
$ErrorActionPreference = 'Stop'
$Root = Split-Path -Parent $PSScriptRoot
$Setup = Join-Path $Root 'release\WordProSuite_Setup.exe'
$Dll = Join-Path $env:LOCALAPPDATA 'WordProSuite\Desktop\WordProSuite.AddIn.dll'
$Log = Join-Path $env:LOCALAPPDATA 'WordProSuite\Logs\WordProSuite.log'

if (-not (Test-Path $Setup)) { throw "Setup not found: $Setup" }
Get-Process WINWORD -ErrorAction SilentlyContinue | Stop-Process -Force

$install = Start-Process $Setup -ArgumentList '/install /silent' -Wait -PassThru
if ($install.ExitCode -ne 0) { throw "Setup install failed: $($install.ExitCode)" }
if (-not (Test-Path $Dll)) { throw "Installed DLL not found: $Dll" }

$word = New-Object -ComObject Word.Application
$word.Visible = $false
$doc = $word.Documents.Add()
$addin = $word.COMAddIns.Item('WordProSuite.Desktop.AddIn')
$addin.Connect = $true
Start-Sleep 3

if (-not $addin.Connect) { throw 'Word did not connect the COM add-in' }
if (-not (Test-Path $Log)) { throw 'Startup log not found' }
if ((Get-Content $Log -Raw) -notmatch 'OnConnection') { throw 'OnConnection not confirmed' }

$doc.Close(0)
$word.Quit()
[Runtime.InteropServices.Marshal]::FinalReleaseComObject($addin) | Out-Null
[Runtime.InteropServices.Marshal]::FinalReleaseComObject($doc) | Out-Null
[Runtime.InteropServices.Marshal]::FinalReleaseComObject($word) | Out-Null

$remove = Start-Process $Setup -ArgumentList '/uninstall /silent' -Wait -PassThru
if ($remove.ExitCode -ne 0) { throw "Setup uninstall failed: $($remove.ExitCode)" }
Write-Host "Office 2024 integration test passed for $OfficeArchitecture"
