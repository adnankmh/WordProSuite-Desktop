$ErrorActionPreference = 'Stop'
$Root = Split-Path -Parent $PSScriptRoot
$Setup = Join-Path $Root 'release\WordProSuite_Setup.exe'
$Log = Join-Path $env:LOCALAPPDATA 'WordProSuite\Logs\WordProSuite.log'

if (-not (Test-Path $Setup)) { throw "Setup not found: $Setup" }

Get-Process WINWORD -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Process $Setup -ArgumentList '/remove /silent' -Wait
Start-Process $Setup -ArgumentList '/install /silent' -Wait

$word = New-Object -ComObject Word.Application
$word.Visible = $false
$doc = $word.Documents.Add()
$addin = $word.COMAddIns.Item('WordProSuite.Desktop.AddIn')
$addin.Connect = $true
Start-Sleep 3

if (-not $addin.Connect) { throw 'Word did not connect the COM add-in.' }
if (-not (Test-Path $Log)) { throw 'Startup log not found.' }
if ((Get-Content $Log -Raw) -notmatch 'OnConnection') { throw 'OnConnection was not confirmed in the log.' }

$doc.Close(0)
$word.Quit()
[Runtime.InteropServices.Marshal]::FinalReleaseComObject($addin) | Out-Null
[Runtime.InteropServices.Marshal]::FinalReleaseComObject($doc) | Out-Null
[Runtime.InteropServices.Marshal]::FinalReleaseComObject($word) | Out-Null

Start-Process $Setup -ArgumentList '/remove /silent' -Wait
Write-Host 'Office 2024 integration test passed for WordPro Suite Desktop Ultimate 3.0.'
