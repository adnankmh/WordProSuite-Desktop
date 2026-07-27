param(
    [ValidateSet('Release','Debug')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$Root = Split-Path -Parent $PSScriptRoot
$Release = Join-Path $Root 'release'
$Payload = Join-Path $Release 'Payload'
$ZipPath = Join-Path $Root 'WordProSuite_Desktop_Pro_V2_Windows.zip'

if (Test-Path $Release) { Remove-Item $Release -Recurse -Force }
if (Test-Path $ZipPath) { Remove-Item $ZipPath -Force }
New-Item $Payload -ItemType Directory -Force | Out-Null

Write-Host 'Restoring solution...'
dotnet restore (Join-Path $Root 'WordProSuite.Desktop.sln')
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed: $LASTEXITCODE" }

Write-Host 'Building solution...'
dotnet build (Join-Path $Root 'WordProSuite.Desktop.sln') -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed: $LASTEXITCODE" }

$AddInDll = Join-Path $Root "src\WordProSuite.AddIn\bin\$Configuration\net48\WordProSuite.AddIn.dll"
$SetupExe = Join-Path $Root "src\WordProSuite.SetupLauncher\bin\$Configuration\net48\WordProSuite_Setup.exe"

if (-not (Test-Path $AddInDll)) { throw "Add-in DLL missing: $AddInDll" }
if (-not (Test-Path $SetupExe)) { throw "Setup EXE missing: $SetupExe" }

Copy-Item $SetupExe (Join-Path $Release 'WordProSuite_Setup.exe') -Force
Copy-Item $AddInDll (Join-Path $Payload 'WordProSuite.AddIn.dll') -Force

$readme = Join-Path $Root 'README_AR.md'
if (Test-Path $readme) { Copy-Item $readme (Join-Path $Release 'README_AR.md') -Force }

$installText = @'
WordPro Suite Desktop Pro v2

1. أغلق Microsoft Word.
2. حافظ على مجلد Payload بجانب WordProSuite_Setup.exe.
3. شغّل WordProSuite_Setup.exe.
4. اضغط «تثبيت».
5. افتح Word ثم استخدم «تفعيل البرنامج» وأدخل Serial Number.

لا ترفع أو توزع ملف المفتاح الخاص بمالك البرنامج.
'@
Set-Content -Path (Join-Path $Release 'INSTALL_AR.txt') -Value $installText -Encoding UTF8

Compress-Archive -Path (Join-Path $Release '*') -DestinationPath $ZipPath -Force

$required = @(
    (Join-Path $Release 'WordProSuite_Setup.exe'),
    (Join-Path $Payload 'WordProSuite.AddIn.dll'),
    $ZipPath
)
foreach ($file in $required) {
    if (-not (Test-Path $file)) { throw "Required release file missing: $file" }
    if ((Get-Item $file).Length -lt 1024) { throw "Required release file is too small: $file" }
}

$hash = (Get-FileHash $ZipPath -Algorithm SHA256).Hash
Set-Content -Path (Join-Path $Release 'SHA256.txt') -Value "$hash  $(Split-Path $ZipPath -Leaf)" -Encoding ASCII
Write-Host "Built successfully: $ZipPath"
