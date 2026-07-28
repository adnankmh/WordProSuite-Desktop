$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$Root = Split-Path -Parent $PSScriptRoot

$required = @(
    'WordProSuite.Desktop.sln',
    'src\WordProSuite.AddIn\WordProSuite.AddIn.csproj',
    'src\WordProSuite.AddIn\Commands\CommandRouter.cs',
    'src\WordProSuite.AddIn\Commands\AdvancedCommands.cs',
    'src\WordProSuite.AddIn\Commands\EnterpriseCommands.cs',
    'src\WordProSuite.AddIn\Commands\ProfessionalCommands.cs',
    'src\WordProSuite.AddIn\Commands\UltimateCommands.cs',
    'src\WordProSuite.AddIn\Commands\Suite600Commands.cs',
    'src\WordProSuite.AddIn\Commands\FeatureReferenceCatalog.cs',
    'src\WordProSuite.AddIn\UI\Prompt.cs',
    'src\WordProSuite.AddIn\UI\Catalog600Form.cs',
    'src\WordProSuite.AddIn\Ribbon\RibbonXml.cs',
    'src\WordProSuite.SetupLauncher\WordProSuite.SetupLauncher.csproj',
    'src\WordProSuite.SetupLauncher\Program.cs',
    'catalog\ultimate_word_suite_600.json',
    'catalog\ultimate_word_suite_600.csv',
    'scripts\build-release.ps1'
)

foreach ($relative in $required) {
    $path = Join-Path $Root $relative
    if (-not (Test-Path $path)) { throw "Required source file missing: $relative" }
}

$routerPath = Join-Path $Root 'src\WordProSuite.AddIn\Commands\CommandRouter.cs'
$router = Get-Content $routerPath -Raw
$idMatches = [regex]::Matches($router, 'A\("([^"]+)"')
$ids = @($idMatches | ForEach-Object { $_.Groups[1].Value })

if ($ids.Count -ne 600) {
    throw "Expected exactly 600 registered commands, found $($ids.Count)."
}

$duplicates = @($ids | Group-Object | Where-Object Count -gt 1)
if ($duplicates.Count -gt 0) {
    throw "Duplicate command IDs: $($duplicates.Name -join ', ')"
}

$catalogPath = Join-Path $Root 'catalog\ultimate_word_suite_600.json'
$catalog = @(Get-Content $catalogPath -Raw | ConvertFrom-Json)
if ($catalog.Count -ne 600) {
    throw "Expected 600 reference catalog entries, found $($catalog.Count)."
}

$catalogNumbers = @($catalog | ForEach-Object { [int]$_.referenceNumber })
$missingNumbers = @(1..600 | Where-Object { $_ -notin $catalogNumbers })
if ($missingNumbers.Count -gt 0) {
    throw "Reference catalog is missing numbers: $($missingNumbers -join ', ')"
}

$catalogIds = @($catalog | ForEach-Object { [string]$_.commandId })
$catalogDuplicateIds = @($catalogIds | Group-Object | Where-Object Count -gt 1)
if ($catalogDuplicateIds.Count -gt 0) {
    throw "Reference catalog contains duplicate command IDs: $($catalogDuplicateIds.Name -join ', ')"
}

$catalogUnknown = @($catalogIds | Where-Object { $_ -notin $ids })
if ($catalogUnknown.Count -gt 0) {
    throw "Reference catalog points to unknown commands: $($catalogUnknown -join ', ')"
}

$engineCount = @($catalog | Select-Object -ExpandProperty engine -Unique).Count
if ($engineCount -ne 15) {
    throw "Expected exactly 15 reference engines, found $engineCount."
}

$ribbonPath = Join-Path $Root 'src\WordProSuite.AddIn\Ribbon\RibbonXml.cs'
$ribbon = Get-Content $ribbonPath -Raw
$tabCount = ([regex]::Matches($ribbon, '<tab\s')).Count
if ($tabCount -ne 3) {
    throw "Expected exactly three professional Ribbon tabs, found $tabCount."
}

$tagMatches = [regex]::Matches($ribbon, 'tag=""([^""]+)""')
$tags = @($tagMatches | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique)
$missingTags = @($tags | Where-Object { $_ -notin $ids })
if ($missingTags.Count -gt 0) {
    throw "Ribbon tags without registered commands: $($missingTags -join ', ')"
}

$commandsFolder = Join-Path $Root 'src\WordProSuite.AddIn\Commands'
$commandFiles = Get-ChildItem $commandsFolder -Filter '*.cs' -File
$knownDynamicLambdaPatterns = @(
    'TextTransforms\.Lines\(Convert\.ToString\(r\.Text\)\)\.Select\s*\(',
    'TextTransforms\.Lines\([^\)]*dynamic[^\)]*\)\.Select\s*\('
)
foreach ($file in $commandFiles) {
    $content = Get-Content $file.FullName -Raw
    foreach ($pattern in $knownDynamicLambdaPatterns) {
        if ([regex]::IsMatch($content, $pattern)) {
            throw "Known CS1977 dynamic/lambda pattern found in $($file.Name)."
        }
    }
}

$enterprisePath = Join-Path $Root 'src\WordProSuite.AddIn\Commands\EnterpriseCommands.cs'
$enterprise = Get-Content $enterprisePath -Raw
if ($enterprise -match 'Prompt\.Show' -and $enterprise -notmatch 'using\s+WordProSuite\.Desktop\.UI\s*;') {
    throw 'EnterpriseCommands.cs uses Prompt.Show without importing WordProSuite.Desktop.UI.'
}

$setupProject = Get-Content (Join-Path $Root 'src\WordProSuite.SetupLauncher\WordProSuite.SetupLauncher.csproj') -Raw
if ($setupProject -notmatch 'EmbeddedResource' -or $setupProject -notmatch 'WordProSuite\.AddIn\.dll') {
    throw 'Setup project is not configured to embed WordProSuite.AddIn.dll.'
}

$setupSource = Get-Content (Join-Path $Root 'src\WordProSuite.SetupLauncher\Program.cs') -Raw
if ($setupSource -notmatch 'GetManifestResourceStream\("WordProSuite\.AddIn\.dll"\)') {
    throw 'Setup source does not extract the embedded add-in payload.'
}
if ($setupSource -notmatch 'VerifyComActivation') {
    throw 'Setup source does not verify COM activation.'
}
if ($setupSource -notmatch 'Ultimate 4\.0') {
    throw 'Setup branding/version was not updated to Ultimate 4.0.'
}
if ($setupSource -match 'إمكانية تشغيلها\.\s*\r?\n\s*بعد التثبيت') {
    throw 'Regression: Setup Program.cs contains a physical newline inside a regular C# string literal.'
}
if ($setupSource -notmatch 'ثلاثة تبويبات احترافية') {
    throw 'Setup description does not advertise the three-tab release.'
}

$buildSource = Get-Content (Join-Path $Root 'scripts\build-release.ps1') -Raw
if ($buildSource -match 'validate-source\.ps1''\)\s*\r?\nif \(\$LASTEXITCODE') {
    throw 'Regression: build script reads LASTEXITCODE after an in-process PowerShell script.'
}

$privatePatterns = @(
    '<P>[^<]+</P>',
    '<Q>[^<]+</Q>',
    '<D>[^<]+</D>',
    'PRIVATE_KEY'
)

$sourceFiles = Get-ChildItem (Join-Path $Root 'src') -Recurse -File |
    Where-Object { $_.Extension -in '.cs','.csproj','.xml','.config' }

foreach ($file in $sourceFiles) {
    $content = Get-Content $file.FullName -Raw
    foreach ($pattern in $privatePatterns) {
        if ([regex]::IsMatch($content, $pattern, [Text.RegularExpressions.RegexOptions]::IgnoreCase)) {
            throw "Potential private licensing key material found in public source: $($file.FullName)"
        }
    }
}

Write-Host '[OK] Required source files'
Write-Host "[OK] Registered commands: $($ids.Count)"
Write-Host '[OK] Unique command IDs'
Write-Host "[OK] Reference catalog: $($catalog.Count) tools / $engineCount engines"
Write-Host "[OK] Ribbon tabs: $tabCount"
Write-Host "[OK] Ribbon command mapping: $($tags.Count) unique tags"
Write-Host '[OK] Prompt namespace/import regression check'
Write-Host '[OK] CS1977 dynamic/lambda regression checks'
Write-Host '[OK] Setup multiline-string regression check'
Write-Host '[OK] Single-file embedded Setup configuration'
Write-Host '[OK] Build-script LASTEXITCODE regression check'
Write-Host '[OK] Private-key leak scan'
