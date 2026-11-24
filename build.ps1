param(
    [string]$GameDir = "",
    [string]$Dotnet = "",
    [switch]$Install
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$project = Join-Path $root "HopperChains.csproj"
$manifestPath = Join-Path $root "manifest.xml"
$readme = Join-Path $root "README.md"
$license = Join-Path $root "LICENSE"
$modName = "HopperChains"

if (-not (Test-Path $manifestPath)) {
    throw "manifest.xml was not found in '$root'."
}
if (-not (Test-Path $readme)) {
    throw "README.md was not found in '$root'."
}
if (-not (Test-Path $license)) {
    throw "LICENSE was not found in '$root'."
}

# Keep the manifest, project, tag, and ZIP version in sync.
[xml]$manifestXml = Get-Content $manifestPath
[xml]$projectXml = Get-Content $project
$manifestVersionNode = $manifestXml.SelectSingleNode("/Manifest/ModVersion")
$projectVersionNode = $projectXml.SelectSingleNode("//Version")
$manifestVersion = if ($manifestVersionNode) { $manifestVersionNode.InnerText.Trim() } else { "" }
$projectVersion = if ($projectVersionNode) { $projectVersionNode.InnerText.Trim() } else { "" }

if (-not $manifestVersion) {
    throw "manifest.xml has no <ModVersion>."
}
if (-not $projectVersion) {
    throw "HopperChains.csproj has no <Version>."
}
if ($manifestVersion -ne $projectVersion) {
    throw "Version mismatch: manifest.xml is '$manifestVersion' but HopperChains.csproj is '$projectVersion'."
}

if (-not $GameDir) {
    if ($env:PROBABLY_STOLEN_DIR) {
        $GameDir = $env:PROBABLY_STOLEN_DIR
    } elseif (Test-Path "E:\SteamLibrary\steamapps\common\Probably Stolen") {
        $GameDir = "E:\SteamLibrary\steamapps\common\Probably Stolen"
    } elseif (Test-Path "C:\Program Files (x86)\Steam\steamapps\common\Probably Stolen") {
        $GameDir = "C:\Program Files (x86)\Steam\steamapps\common\Probably Stolen"
    } else {
        throw "Could not find Probably Stolen. Pass -GameDir `"<path to game>`" or set PROBABLY_STOLEN_DIR."
    }
}

$managed = Join-Path $GameDir "Probably Stolen_Data\Managed"
if (-not (Test-Path (Join-Path $managed "Assembly-CSharp.dll"))) {
    throw "Assembly-CSharp.dll was not found in '$managed'. Check -GameDir."
}

if (-not $Dotnet) {
    $candidates = @(
        (Join-Path $env:USERPROFILE "AppData\Local\Microsoft\dotnet\dotnet.exe"),
        (Join-Path $env:ProgramFiles "dotnet\dotnet.exe")
    )
    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) {
            $Dotnet = $candidate
            break
        }
    }
}

if (-not $Dotnet) {
    $command = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($command) {
        $Dotnet = $command.Source
    }
}

if (-not $Dotnet -or -not (Test-Path $Dotnet)) {
    throw "dotnet was not found. Install the .NET 8 SDK or pass -Dotnet `"<path to dotnet.exe>`"."
}

Write-Host "Building Hopper Chains..."
& $Dotnet build $project -c Release --nologo "-p:GameDir=$GameDir"
if ($LASTEXITCODE -ne 0) {
    throw "dotnet build failed with exit code $LASTEXITCODE."
}

$assembly = Join-Path $root "bin\Release\netstandard2.1\$modName.dll"
if (-not (Test-Path $assembly)) {
    throw "Build succeeded but '$assembly' was not produced."
}

$distRoot = Join-Path $root "dist"
$dist = Join-Path $distRoot $modName
$archive = Join-Path $distRoot "$modName-v$manifestVersion.zip"

if (Test-Path $distRoot) {
    Remove-Item $distRoot -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $dist | Out-Null
Copy-Item $assembly $dist
Copy-Item $manifestPath $dist
Copy-Item $readme $dist
Copy-Item $license $dist

Compress-Archive -Path $dist -DestinationPath $archive -CompressionLevel Optimal

Write-Host "Package: $dist"
Write-Host "Release archive: $archive"

if ($Install) {
    $target = Join-Path $GameDir "Mods\$modName"
    if (Test-Path $target) {
        Remove-Item $target -Recurse -Force
    }

    New-Item -ItemType Directory -Force -Path $target | Out-Null
    Copy-Item (Join-Path $dist "*") $target -Recurse
    Write-Host "Installed to: $target"
    Write-Host "Enable/trust 'Hopper Chains' in the Mods screen, then restart the game."
}
