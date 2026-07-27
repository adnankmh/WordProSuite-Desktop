$ErrorActionPreference = "Stop"

Write-Host "Closing Microsoft Word..."
Get-Process WINWORD -ErrorAction SilentlyContinue | Stop-Process -Force

$progId = "WordProSuite.Desktop.AddIn"
$officeAddin = "HKCU:\Software\Microsoft\Office\Word\Addins\$progId"

Write-Host "Resetting Word add-in state..."
New-Item -Path $officeAddin -Force | Out-Null
New-ItemProperty -Path $officeAddin -Name FriendlyName -Value "WordPro Suite Desktop" -PropertyType String -Force | Out-Null
New-ItemProperty -Path $officeAddin -Name Description -Value "Professional productivity tools for Microsoft Word." -PropertyType String -Force | Out-Null
New-ItemProperty -Path $officeAddin -Name LoadBehavior -Value 3 -PropertyType DWord -Force | Out-Null
New-ItemProperty -Path $officeAddin -Name CommandLineSafe -Value 0 -PropertyType DWord -Force | Out-Null

# Clear Word's cached crash/disable records. These keys are rebuilt by Office.
$wordResiliency = "HKCU:\Software\Microsoft\Office\16.0\Word\Resiliency"
if (Test-Path $wordResiliency) {
    Remove-Item "$wordResiliency\DisabledItems" -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item "$wordResiliency\CrashingAddinList" -Recurse -Force -ErrorAction SilentlyContinue
}

$install = Join-Path ${env:ProgramFiles} "WordPro Suite\WordPro Suite Desktop\WordProSuite.AddIn.dll"
if (-not (Test-Path $install) -and ${env:ProgramFiles(x86)}) {
    $install = Join-Path ${env:ProgramFiles(x86)} "WordPro Suite\WordPro Suite Desktop\WordProSuite.AddIn.dll"
}

if (-not (Test-Path $install)) {
    throw "WordProSuite.AddIn.dll was not found. Install the MSI first."
}

$is64Office = (Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\Office\ClickToRun\Configuration" -ErrorAction SilentlyContinue).Platform -eq "x64"
$regasm = if ($is64Office) {
    "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe"
} else {
    "$env:WINDIR\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe"
}

Write-Host "Registering managed COM server with: $regasm"
& $regasm $install /nologo /codebase
if ($LASTEXITCODE -ne 0) { throw "RegAsm failed with exit code $LASTEXITCODE" }

Write-Host ""
Write-Host "Repair completed. Open Word now."
Read-Host "Press Enter to close"
