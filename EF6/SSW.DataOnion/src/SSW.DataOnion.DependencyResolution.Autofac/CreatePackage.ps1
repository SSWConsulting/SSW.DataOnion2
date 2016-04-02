$0 = $myInvocation.MyCommand.Definition
$dp0 = [System.IO.Path]::GetDirectoryName($0)

cd $dp0

echo $dp0

echo "Update package number"
(Get-Content Properties\AssemblyInfo.cs) | 
Foreach-Object { $n = [regex]::match($_, '\[assembly: AssemblyVersion\(\"\d+\.\d+\.(\d+)\"\)\]').groups[1].value; if ($n) { $ver=([int32]$n+1);  $_ -replace '\[assembly: AssemblyVersion\(\"(\d+)\.(\d+)\.(\d+)\"\)\]', ('[assembly: AssemblyVersion("$1.$2.' + $ver + '")]')} else {$_}; } | 
Set-Content Properties\AssemblyInfo.cs

echo "Cleaning up old packages..."
del *.nupkg

echo "Building and packaging new version of package..."
nuget pack SSW.DataOnion.DependencyResolution.Autofac.csproj -Prop Configuration=Release -IncludeReferencedProjects