param (
    [string]$packageToDelete = $( Read-Host "Please provide the package to delete" )
)

$nugetFeed = "https://api.nuget.org/v3/index.json"
$nugetExe = ".\nuget.exe"

function Get-PackageVersions
{
    Param
    (
        [string]$Package,
        [string]$Feed,
        [string]$Nuget
    )

    $packageContent = & $Nuget list -Source $Feed -PreRelease -AllVersions
    $packages = $packageContent.Split([Environment]::NewLine)
    foreach($line in $packages) {
        $parts = $line.Split(' ')
        $packageName = $parts[0]
        $packageVersion = $parts[1]
        if($packageName -eq $Package) {
            $packageVersion
        }
    }
}

$versions = Get-PackageVersions -Package $packageToDelete -Feed $nugetFeed -Nuget $nugetExe
foreach($version in $versions) {
    & $nugetExe delete $packageToDelete $version -Source $nugetFeed
}