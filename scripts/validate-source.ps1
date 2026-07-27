$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$Root = Split-Path -Parent $PSScriptRoot

$required = @(
    'WordProSuite.Desktop.sln',
    'src\WordProSuite.AddIn\WordProSuite.AddIn.csproj',
    'src\WordProSuite.AddIn\Commands\CommandRouter.cs',
    'src\WordProSuite.AddIn\Commands\AdvancedCommands.cs',
    'src\WordProSuite.AddIn\Commands\EnterpriseCommands.cs',
    'src\WordProSuite.AddIn\Ribbon\RibbonXml.cs',
    'src\WordProSuite.SetupLauncher\WordProSuite.SetupLauncher.csproj',
    'src\WordProSuite.SetupLauncher\Program.cs'
)

foreach ($relative in $required) {
    $path = Join-Path $Root $relative
    if (-not (Test-Path $path)) { throw "Required source file missing: $relative" }
}

$routerPath = Join-Path $Root 'src\WordProSuite.AddIn\Commands\CommandRouter.cs'
$router = Get-Content $routerPath -Raw
$idMatches = [regex]::Matches($router, 'A\("([^"]+)"')
$ids = @($idMatches | ForEach-Object { $_.Groups[1].Value })

if ($ids.Count -lt 250) {
    throw "Expected at least 250 registered commands, found $($ids.Count)."
}

$duplicates = @($ids | Group-Object | Where-Object Count -gt 1)
if ($duplicates.Count -gt 0) {
    throw "Duplicate command IDs: $($duplicates.Name -join ', ')"
}

$advancedPath = Join-Path $Root 'src\WordProSuite.AddIn\Commands\AdvancedCommands.cs'
$advanced = Get-Content $advancedPath -Raw

$knownDynamicLambdaPattern = 'TextTransforms\.Lines\(Convert\.ToString\(r\.Text\)\)\.Select\s*\('
if ([regex]::IsMatch($advanced, $knownDynamicLambdaPattern)) {
    throw 'Known CS1977 dynamic/lambda pattern still exists in AdvancedCommands.cs.'
}

$ribbonPath = Join-Path $Root 'src\WordProSuite.AddIn\Ribbon\RibbonXml.cs'
$ribbon = Get-Content $ribbonPath -Raw
$tagMatches = [regex]::Matches($ribbon, 'tag=""([^""]+)""')
$tags = @($tagMatches | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique)
$missingTags = @($tags | Where-Object { $_ -notin $ids })

if ($missingTags.Count -gt 0) {
    throw "Ribbon tags without registered commands: $($missingTags -join ', ')"
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

Write-Host "[OK] Required source files"
Write-Host "[OK] Registered commands: $($ids.Count)"
Write-Host "[OK] Unique command IDs"
Write-Host "[OK] Ribbon command mapping: $($tags.Count) tags"
Write-Host "[OK] CS1977 dynamic/lambda regression check"
Write-Host "[OK] Single-file embedded Setup configuration"
Write-Host "[OK] Private-key leak scan"
