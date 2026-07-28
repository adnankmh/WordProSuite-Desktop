param(
    [ValidateSet('Release','Debug')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$Root = Split-Path -Parent $PSScriptRoot
$Release = Join-Path $Root 'release'
$EmbeddedPayload = Join-Path $Root 'src\WordProSuite.SetupLauncher\EmbeddedPayload'
$ZipPath = Join-Path $Root 'WordProSuite_Desktop_Ultimate_V4_Windows.zip'

if (Test-Path $Release) { Remove-Item $Release -Recurse -Force }
if (Test-Path $EmbeddedPayload) { Remove-Item $EmbeddedPayload -Recurse -Force }
if (Test-Path $ZipPath) { Remove-Item $ZipPath -Force }

New-Item $Release -ItemType Directory -Force | Out-Null
New-Item $EmbeddedPayload -ItemType Directory -Force | Out-Null

Write-Host 'Validating source...'
try {
    & (Join-Path $Root 'scripts\validate-source.ps1')
}
catch {
    throw "Source validation failed: $($_.Exception.Message)"
}

Write-Host 'Restoring projects...'
dotnet restore (Join-Path $Root 'WordProSuite.Desktop.sln')
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed: $LASTEXITCODE" }

Write-Host 'Building Word add-in with warnings treated as errors...'
dotnet build (Join-Path $Root 'src\WordProSuite.AddIn\WordProSuite.AddIn.csproj') `
    -c $Configuration --no-restore -warnaserror
if ($LASTEXITCODE -ne 0) { throw "Add-in build failed: $LASTEXITCODE" }

$AddInDll = Join-Path $Root "src\WordProSuite.AddIn\bin\$Configuration\net48\WordProSuite.AddIn.dll"
if (-not (Test-Path $AddInDll)) { throw "Add-in DLL missing: $AddInDll" }
if ((Get-Item $AddInDll).Length -lt 4096) { throw 'Add-in DLL is unexpectedly small.' }

Copy-Item $AddInDll (Join-Path $EmbeddedPayload 'WordProSuite.AddIn.dll') -Force

Write-Host 'Rebuilding single-file Setup.exe with embedded add-in payload...'
dotnet build (Join-Path $Root 'src\WordProSuite.SetupLauncher\WordProSuite.SetupLauncher.csproj') `
    -c $Configuration --no-restore -t:Rebuild -warnaserror
if ($LASTEXITCODE -ne 0) { throw "Setup build failed: $LASTEXITCODE" }

$SetupExe = Join-Path $Root "src\WordProSuite.SetupLauncher\bin\$Configuration\net48\WordProSuite_Setup.exe"
if (-not (Test-Path $SetupExe)) { throw "Setup EXE missing: $SetupExe" }
if ((Get-Item $SetupExe).Length -le (Get-Item $AddInDll).Length) {
    throw 'Setup EXE does not appear to contain the embedded add-in payload.'
}

$setupAssembly = [Reflection.Assembly]::LoadFile($SetupExe)
$resourceNames = @($setupAssembly.GetManifestResourceNames())
if ('WordProSuite.AddIn.dll' -notin $resourceNames) {
    throw "Embedded add-in resource missing from Setup. Resources: $($resourceNames -join ', ')"
}

$ReleaseSetup = Join-Path $Release 'WordProSuite_Setup.exe'
Copy-Item $SetupExe $ReleaseSetup -Force

$install = @'
WordPro Suite Desktop Ultimate 4.0

طريقة الاستخدام:
1. أغلق Microsoft Word.
2. شغّل WordProSuite_Setup.exe فقط.
3. للتجربة اضغط «تثبيت تجريبي».
4. للتفعيل المباشر الصق Serial Number واضغط «تثبيت وتفعيل».
5. افتح Word؛ ستظهر ثلاثة تبويبات: WordPro Suite Pro وWordPro Enterprise وUltra 600 AI.

النسخة تحتوي على 600 أداة مسجلة ومقسمة على 15 محركًا، ولا تحتاج إلى Payload أو MSI أو Node.js أو localhost.
'@
Set-Content -Path (Join-Path $Release 'INSTALL_AR.txt') -Value $install -Encoding UTF8

$features = Join-Path $Root 'V4_FEATURES_AR.md'
if (Test-Path $features) {
    Copy-Item $features (Join-Path $Release 'FEATURES_AR.md') -Force
}

$readme = Join-Path $Root 'README_AR.md'
if (Test-Path $readme) {
    Copy-Item $readme (Join-Path $Release 'README_AR.md') -Force
}

Copy-Item (Join-Path $Root 'catalog\ultimate_word_suite_600.json') (Join-Path $Release 'ULTIMATE_600_CATALOG.json') -Force
Copy-Item (Join-Path $Root 'catalog\ultimate_word_suite_600.csv') (Join-Path $Release 'ULTIMATE_600_CATALOG.csv') -Force

$hash = (Get-FileHash $ReleaseSetup -Algorithm SHA256).Hash
Set-Content -Path (Join-Path $Release 'SHA256_SETUP.txt') `
    -Value "$hash  WordProSuite_Setup.exe" -Encoding ASCII

$manifest = [ordered]@{
    product = 'WordPro Suite Desktop Ultimate'
    version = '4.0.0'
    commands = 600
    ribbonTabs = 3
    setup = 'WordProSuite_Setup.exe'
    setupSha256 = $hash
    generatedUtc = [DateTime]::UtcNow.ToString('o')
}
$manifest | ConvertTo-Json -Depth 4 | Set-Content (Join-Path $Release 'RELEASE_MANIFEST.json') -Encoding UTF8

Compress-Archive -Path (Join-Path $Release '*') -DestinationPath $ZipPath -Force

$required = @(
    $ReleaseSetup,
    (Join-Path $Release 'INSTALL_AR.txt'),
    (Join-Path $Release 'SHA256_SETUP.txt'),
    (Join-Path $Release 'RELEASE_MANIFEST.json'),
    (Join-Path $Release 'ULTIMATE_600_CATALOG.json'),
    (Join-Path $Release 'ULTIMATE_600_CATALOG.csv'),
    $ZipPath
)

foreach ($file in $required) {
    if (-not (Test-Path $file)) { throw "Required release file missing: $file" }
    if ((Get-Item $file).Length -lt 32) { throw "Required release file is too small: $file" }
}

Write-Host ''
Write-Host '============================================================'
Write-Host 'WordPro Suite Desktop Ultimate 4.0 build completed successfully.'
Write-Host 'Registered tools: 600'
Write-Host 'Ribbon tabs:      3'
Write-Host "Setup:            $ReleaseSetup"
Write-Host "ZIP:              $ZipPath"
Write-Host '============================================================'
