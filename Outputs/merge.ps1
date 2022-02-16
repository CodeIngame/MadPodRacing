#cat ..\OceanOfCode\*.cs ..\OceanOfCode\*\*.cs | sc merge.cs

Write-Host 'start merging'

$projectName = "MadPodRacing"
$proPath = "C:\Projets\Perso\CodingGame"
$personnalPath = "E:\Projets\Visual Studio 2022"

$pathToUse = $personnalPath
#$pathToUse = $proPath


$fromPath = "$pathToUse\$projectName\$projectName.Domain\*.cs"
$toFile = "$pathToUse\$projectName\Outputs\output.cs"

Write-Host "from -> $fromPath"
Write-Host "to <- $toFile"


Get-ChildItem -Recurse $fromPath | Where {$_.Name -like "*.cs" -and $_.Name -notlike "*.Assembly*.cs" } | ForEach-Object { Get-Content $_ } | Out-File $toFile


Write-Host 'done merging'

# Get-ChildItem -Recurse "C:\Users\Q z L\Documents\Visual Studio 2019\Projects\CodeInGame\OceanOfCode\OceanOfCode\*.cs"