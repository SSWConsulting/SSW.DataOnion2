.\CreatePackage.ps1

$0 = $myInvocation.MyCommand.Definition
$dp0 = [System.IO.Path]::GetDirectoryName($0)

cd $dp0

echo "Pushing new package to server..."
$content = [IO.File]::ReadAllText($dp0 + "\..\..\..\..\api.txt")
nuget push *.nupkg -ApiKey $content