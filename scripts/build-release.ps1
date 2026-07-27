param(
    [ValidateSet('Release','Debug')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$Root = Split-Path -Parent $PSScriptRoot
$Release = Join-Path $Root 'release'
$EmbeddedPayload = Join-Path $Root 'src\WordProSuite.SetupLauncher\EmbeddedPayload'
$ZipPath = Join-Path $Root 'WordProSuite_Desktop_Pro_V2_1_Windows.zip'

if (Test-Path $Release) { Remove-Item $Release -Recurse -Force }
if (Test-Path $EmbeddedPayload) { Remove-Item $EmbeddedPayload -Recurse -Force }
if (Test-Path $ZipPath) { Remove-Item $ZipPath -Force }

New-Item $Release -ItemType Directory -Force | Out-Null
New-Item $EmbeddedPayload -ItemType Directory -Force | Out-Null

Write-Host 'Validating source...'
& (Join-Path $Root 'scripts\validate-source.ps1')
if ($LASTEXITCODE -ne 0) { throw "Source validation failed: $LASTEXITCODE" }

Write-Host 'Restoring projects...'
dotnet restore (Join-Path $Root 'WordProSuite.Desktop.sln')
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed: $LASTEXITCODE" }

Write-Host 'Building Word add-in...'
dotnet build (Join-Path $Root 'src\WordProSuite.AddIn\WordProSuite.AddIn.csproj') `
    -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { throw "Add-in build failed: $LASTEXITCODE" }

$AddInDll = Join-Path $Root "src\WordProSuite.AddIn\bin\$Configuration\net48\WordProSuite.AddIn.dll"
if (-not (Test-Path $AddInDll)) { throw "Add-in DLL missing: $AddInDll" }
if ((Get-Item $AddInDll).Length -lt 4096) { throw "Add-in DLL is unexpectedly small." }

Copy-Item $AddInDll (Join-Path $EmbeddedPayload 'WordProSuite.AddIn.dll') -Force

Write-Host 'Building single-file Setup.exe with embedded add-in payload...'
dotnet build (Join-Path $Root 'src\WordProSuite.SetupLauncher\WordProSuite.SetupLauncher.csproj') `
    -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { throw "Setup build failed: $LASTEXITCODE" }

$SetupExe = Join-Path $Root "src\WordProSuite.SetupLauncher\bin\$Configuration\net48\WordProSuite_Setup.exe"
if (-not (Test-Path $SetupExe)) { throw "Setup EXE missing: $SetupExe" }
if ((Get-Item $SetupExe).Length -le (Get-Item $AddInDll).Length) {
    throw "Setup EXE does not appear to contain the embedded add-in payload."
}

$ReleaseSetup = Join-Path $Release 'WordProSuite_Setup.exe'
Copy-Item $SetupExe $ReleaseSetup -Force

$install = @'
WordPro Suite Desktop Pro 2.1

طريقة الاستخدام:
1. أغلق Microsoft Word.
2. شغّل WordProSuite_Setup.exe فقط.
3. للتجربة اضغط «تثبيت تجريبي».
4. للتفعيل المباشر الصق Serial Number واضغط «تثبيت وتفعيل».
5. افتح Word؛ ستظهر تبويبة WordPro Suite Desktop Pro.

لا يحتاج البرنامج إلى Payload أو MSI أو Node.js أو localhost.
'@
Set-Content -Path (Join-Path $Release 'INSTALL_AR.txt') -Value $install -Encoding UTF8

$features = Join-Path $Root 'V2_1_FEATURES_AR.md'
if (Test-Path $features) {
    Copy-Item $features (Join-Path $Release 'FEATURES_AR.md') -Force
}

$readme = Join-Path $Root 'README_AR.md'
if (Test-Path $readme) {
    Copy-Item $readme (Join-Path $Release 'README_AR.md') -Force
}

$hash = (Get-FileHash $ReleaseSetup -Algorithm SHA256).Hash
Set-Content -Path (Join-Path $Release 'SHA256_SETUP.txt') `
    -Value "$hash  WordProSuite_Setup.exe" -Encoding ASCII

Compress-Archive -Path (Join-Path $Release '*') -DestinationPath $ZipPath -Force

$required = @(
    $ReleaseSetup,
    (Join-Path $Release 'INSTALL_AR.txt'),
    (Join-Path $Release 'SHA256_SETUP.txt'),
    $ZipPath
)

foreach ($file in $required) {
    if (-not (Test-Path $file)) { throw "Required release file missing: $file" }
    if ((Get-Item $file).Length -lt 32) { throw "Required release file is too small: $file" }
}

Write-Host ''
Write-Host '============================================================'
Write-Host 'WordPro Suite Desktop Pro 2.1 build completed successfully.'
Write-Host "Setup: $ReleaseSetup"
Write-Host "ZIP:   $ZipPath"
Write-Host '============================================================'
