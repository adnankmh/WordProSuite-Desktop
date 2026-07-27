param([ValidateSet("Release","Debug")][string]$Configuration="Release")
$ErrorActionPreference="Stop"
$Root=Split-Path -Parent $PSScriptRoot
$Release=Join-Path $Root "release"
$Installers=Join-Path $Release "Installers"
if(Test-Path $Release){Remove-Item $Release -Recurse -Force}
New-Item $Installers -ItemType Directory -Force|Out-Null

dotnet restore "$Root\WordProSuite.Desktop.sln"
dotnet build "$Root\WordProSuite.Desktop.sln" -c $Configuration --no-restore

$AddInBin=Join-Path $Root "src\WordProSuite.AddIn\bin\$Configuration\net48"
$LauncherBin=Join-Path $Root "src\WordProSuite.SetupLauncher\bin\$Configuration\net48"
if(-not(Test-Path "$AddInBin\WordProSuite.AddIn.dll")){throw "Add-in DLL missing"}

$Candle=Get-Command candle.exe -ErrorAction SilentlyContinue
$Light=Get-Command light.exe -ErrorAction SilentlyContinue
if(-not $Candle -or -not $Light){throw "WiX Toolset 3.14.1 is required"}

& $Candle.Source "$Root\installer\x86\Product.wxs" -dAddInBin="$AddInBin" -dProjectRoot="$Root" -arch x86 -out "$Release\WordProSuite.x86.wixobj"
& $Light.Source "$Release\WordProSuite.x86.wixobj" -out "$Installers\WordProSuite.Desktop.x86.msi"

& $Candle.Source "$Root\installer\x64\Product.wxs" -dAddInBin="$AddInBin" -dProjectRoot="$Root" -arch x64 -out "$Release\WordProSuite.x64.wixobj"
& $Light.Source "$Release\WordProSuite.x64.wixobj" -out "$Installers\WordProSuite.Desktop.x64.msi"

Copy-Item "$LauncherBin\WordProSuite_Setup.exe" "$Release\WordProSuite_Setup.exe"
Copy-Item "$Root\README_AR.md" "$Release\README_AR.md"
Compress-Archive -Path "$Release\*" -DestinationPath "$Root\WordProSuite_Desktop_V1_Windows.zip" -Force
Write-Host "Built: $Root\WordProSuite_Desktop_V1_Windows.zip"
