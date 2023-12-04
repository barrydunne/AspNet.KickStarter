$cwd = Get-Location
Set-Location $PSScriptRoot

# Check dotnet-reportgenerator-globaltool is installed
dotnet tool list -g dotnet-reportgenerator-globaltool | Out-Null
if ($LastExitCode -ne 0) {
    dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.1.26
}

dotnet test .\AspNet.KickStarter.Tests\AspNet.KickStarter.Tests.csproj -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/TestResults/ak.xml /p:Include=[AspNet.KickStarter]* /p:Threshold=100
dotnet test .\AspNet.KickStarter.CQRS.Tests\AspNet.KickStarter.CQRS.Tests.csproj -c Release --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/TestResults/cqrs.xml /p:Include=[AspNet.KickStarter.CQRS]* /p:Threshold=100

reportgenerator -reports:'TestResults/ak.xml;TestResults/cqrs.xml' -targetdir:TestResults/CoverageReport -reporttypes:Html_Dark
./TestResults/CoverageReport/index.html

Set-Location $cwd