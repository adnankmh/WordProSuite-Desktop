$ErrorActionPreference = 'Stop'
$Root = Split-Path -Parent $PSScriptRoot

$required = @(
    'src\WordProSuite.AddIn\Commands\CommandRouter.cs',
    'src\WordProSuite.AddIn\Commands\AdvancedCommands.cs',
    'src\WordProSuite.AddIn\Licensing\LicenseManager.cs',
    'src\WordProSuite.AddIn\Ribbon\RibbonXml.cs',
    'src\WordProSuite.SetupLauncher\Program.cs',
    'scripts\build-release.ps1'
)
foreach ($relative in $required) {
    $path = Join-Path $Root $relative
    if (-not (Test-Path $path)) { throw "Required source missing: $relative" }
}

$privateLeaks = Get-ChildItem $Root -Recurse -File | Where-Object {
    $_.Name -match 'PRIVATE_KEY|Owner_License|wps_private'
}
if ($privateLeaks) { throw "Private licensing material must not be committed: $($privateLeaks.FullName -join ', ')" }

$router = Get-Content (Join-Path $Root 'src\WordProSuite.AddIn\Commands\CommandRouter.cs') -Raw
$ids = [regex]::Matches($router, 'A\("([^"]+)"') | ForEach-Object { $_.Groups[1].Value }
if ($ids.Count -lt 180) { throw "Expected at least 180 registered commands; found $($ids.Count)" }
$duplicates = $ids | Group-Object | Where-Object Count -gt 1
if ($duplicates) { throw "Duplicate command IDs: $($duplicates.Name -join ', ')" }

$ribbon = Get-Content (Join-Path $Root 'src\WordProSuite.AddIn\Ribbon\RibbonXml.cs') -Raw
$tags = [regex]::Matches($ribbon, 'tag=""([^""]+)""') | ForEach-Object { $_.Groups[1].Value } | Select-Object -Unique
$missing = $tags | Where-Object { $_ -notin $ids }
if ($missing) { throw "Ribbon tags are not registered: $($missing -join ', ')" }

$publicKey = Get-Content (Join-Path $Root 'src\WordProSuite.AddIn\Licensing\LicenseManager.cs') -Raw
if ($publicKey -notmatch '<RSAKeyValue><Modulus>') { throw 'Public verification key is missing.' }
if ($publicKey -match '<D>') { throw 'Private RSA key leaked into the add-in source.' }

Write-Host "[PASS] Required v2 files"
Write-Host "[PASS] No private key material in repository"
Write-Host "[PASS] $($ids.Count) unique commands"
Write-Host "[PASS] $($tags.Count) Ribbon commands mapped"
Write-Host "[PASS] Public-key-only license verification"
