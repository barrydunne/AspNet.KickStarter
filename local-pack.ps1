$packages = Join-Path $PSScriptRoot "packages"
dotnet pack /p:PackageVersion=9.9.9 /p:AssemblyVersion=9.9.9 /p:FileVersion=9.9.9 /p:Copyright=copyright -c Release AspNet.KickStarter.sln --output $packages
dotnet nuget add source $packages -n AspNet.KickStarter