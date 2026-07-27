param([ValidateSet("Release","Debug")][string]$Configuration="Release")
$ErrorActionPreference="Stop"
Set-StrictMode -Version Latest
$Root=Split-Path -Parent $PSScriptRoot
$Release=Join-Path $Root "release"
$Installers=Join-Path $Release "Installers"
$OneClick=Join-Path $Release "OneClick"
$Payload=Join-Path $OneClick "Payload"
$ZipPath=Join-Path $Root "WordProSuite_Desktop_V1_Windows.zip"
if(Test-Path $Release){Remove-Item $Release -Recurse -Force}
if(Test-Path $ZipPath){Remove-Item $ZipPath -Force}
New-Item $Installers -ItemType Directory -Force|Out-Null
New-Item $Payload -ItemType Directory -Force|Out-Null
dotnet restore (Join-Path $Root "WordProSuite.Desktop.sln")
if($LASTEXITCODE-ne 0){throw "restore failed"}
dotnet build (Join-Path $Root "WordProSuite.Desktop.sln") -c $Configuration --no-restore
if($LASTEXITCODE-ne 0){throw "build failed"}
$AddInBin=Join-Path $Root "src\WordProSuite.AddIn\bin\$Configuration\net48"
$LauncherBin=Join-Path $Root "src\WordProSuite.SetupLauncher\bin\$Configuration\net48"
$AddInDll=Join-Path $AddInBin "WordProSuite.AddIn.dll"
$LauncherExe=Join-Path $LauncherBin "WordProSuite_Setup.exe"
function Tool($n){$c=Get-Command $n -ErrorAction SilentlyContinue;if($c){return $c.Source};if($env:WIX_BIN){$p=Join-Path $env:WIX_BIN $n;if(Test-Path $p){return $p}};throw "$n not found"}
$candle=Tool "candle.exe";$light=Tool "light.exe"
$x86obj=Join-Path $Release "x86.wixobj";$x64obj=Join-Path $Release "x64.wixobj"
$x86msi=Join-Path $Installers "WordProSuite.Desktop.x86.msi";$x64msi=Join-Path $Installers "WordProSuite.Desktop.x64.msi"
&$candle (Join-Path $Root "installer\x86\Product.wxs") "-dAddInBin=$AddInBin" -arch x86 -out $x86obj
if($LASTEXITCODE-ne 0){throw "candle x86 failed"}
&$light $x86obj -out $x86msi
if($LASTEXITCODE-ne 0){throw "light x86 failed"}
&$candle (Join-Path $Root "installer\x64\Product.wxs") "-dAddInBin=$AddInBin" -arch x64 -out $x64obj
if($LASTEXITCODE-ne 0){throw "candle x64 failed"}
&$light $x64obj -out $x64msi
if($LASTEXITCODE-ne 0){throw "light x64 failed"}
Copy-Item $LauncherExe (Join-Path $Release "WordProSuite_Setup.exe") -Force
Copy-Item $AddInDll (Join-Path $Payload "WordProSuite.AddIn.dll") -Force
Copy-Item (Join-Path $Root "installer\portable\Install-WordProSuite.ps1") $OneClick -Force
Copy-Item (Join-Path $Root "installer\portable\INSTALL_WORDPROSUITE.cmd") $OneClick -Force
Copy-Item (Join-Path $Root "installer\portable\Uninstall-WordProSuite.ps1") $OneClick -Force
Copy-Item (Join-Path $Root "installer\portable\UNINSTALL_WORDPROSUITE.cmd") $OneClick -Force
Compress-Archive -Path (Join-Path $Release "*") -DestinationPath $ZipPath -Force
