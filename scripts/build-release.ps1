param(
    [ValidateSet('Release','Debug')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$Root = Split-Path -Parent $PSScriptRoot
$Release = Join-Path $Root 'release'
$Installers = Join-Path $Release 'Installers'
$ZipPath = Join-Path $Root 'WordProSuite_Desktop_V1_Windows.zip'

if (Test-Path $Release) { Remove-Item $Release -Recurse -Force }
if (Test-Path $ZipPath) { Remove-Item $ZipPath -Force }
New-Item $Installers -ItemType Directory -Force | Out-Null

Write-Host 'Restoring solution...'
dotnet restore (Join-Path $Root 'WordProSuite.Desktop.sln')
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed: $LASTEXITCODE" }

Write-Host 'Building solution...'
dotnet build (Join-Path $Root 'WordProSuite.Desktop.sln') -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed: $LASTEXITCODE" }

$AddInBin = Join-Path $Root "src\WordProSuite.AddIn\bin\$Configuration\net48"
$LauncherBin = Join-Path $Root "src\WordProSuite.SetupLauncher\bin\$Configuration\net48"
$AddInDll = Join-Path $AddInBin 'WordProSuite.AddIn.dll'
$LauncherExe = Join-Path $LauncherBin 'WordProSuite_Setup.exe'

if (-not (Test-Path $AddInDll)) { throw "Add-in DLL missing: $AddInDll" }
if (-not (Test-Path $LauncherExe)) { throw "Setup launcher missing: $LauncherExe" }

function Resolve-Tool([string]$Name) {
    $cmd = Get-Command $Name -ErrorAction SilentlyContinue
    if ($cmd) { return $cmd.Source }

    if ($env:WIX_BIN) {
        $candidate = Join-Path $env:WIX_BIN $Name
        if (Test-Path $candidate) { return $candidate }
    }

    throw "$Name was not found. WiX Toolset 3 is required."
}

$Candle = Resolve-Tool 'candle.exe'
$Light = Resolve-Tool 'light.exe'
Write-Host "Using candle: $Candle"
Write-Host "Using light:  $Light"

$x86Obj = Join-Path $Release 'WordProSuite.x86.wixobj'
$x64Obj = Join-Path $Release 'WordProSuite.x64.wixobj'
$x86Msi = Join-Path $Installers 'WordProSuite.Desktop.x86.msi'
$x64Msi = Join-Path $Installers 'WordProSuite.Desktop.x64.msi'

& $Candle (Join-Path $Root 'installer\x86\Product.wxs') "-dAddInBin=$AddInBin" "-dProjectRoot=$Root" -arch x86 -out $x86Obj
if ($LASTEXITCODE -ne 0) { throw "WiX candle x86 failed: $LASTEXITCODE" }
& $Light $x86Obj -out $x86Msi
if ($LASTEXITCODE -ne 0) { throw "WiX light x86 failed: $LASTEXITCODE" }

& $Candle (Join-Path $Root 'installer\x64\Product.wxs') "-dAddInBin=$AddInBin" "-dProjectRoot=$Root" -arch x64 -out $x64Obj
if ($LASTEXITCODE -ne 0) { throw "WiX candle x64 failed: $LASTEXITCODE" }
& $Light $x64Obj -out $x64Msi
if ($LASTEXITCODE -ne 0) { throw "WiX light x64 failed: $LASTEXITCODE" }

Copy-Item $LauncherExe (Join-Path $Release 'WordProSuite_Setup.exe') -Force
Copy-Item (Join-Path $Root 'README_AR.md') (Join-Path $Release 'README_AR.md') -Force

Compress-Archive -Path (Join-Path $Release '*') -DestinationPath $ZipPath -Force

$required = @(
    (Join-Path $Release 'WordProSuite_Setup.exe'),
    $x86Msi,
    $x64Msi,
    $ZipPath
)
foreach ($file in $required) {
    if (-not (Test-Path $file)) { throw "Required release file missing: $file" }
    if ((Get-Item $file).Length -lt 1024) { throw "Required release file is too small: $file" }
}

Write-Host "Built successfully: $ZipPath"
