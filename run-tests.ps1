$cwd = Get-Location
try {
    Set-Location $PSScriptRoot

    # Check dotnet-reportgenerator-globaltool is installed
    dotnet tool list -g dotnet-reportgenerator-globaltool | Out-Null
    if ($LastExitCode -ne 0) {
        dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.1.26
    }

    # Build all *.Tests.csproj files in subfolders
    $testProjects = Get-ChildItem -Path . -Filter "*.Tests.csproj" -Recurse
    foreach ($project in $testProjects) {
        Write-Host ''
        Write-Host "Building $($project.FullName)"
        Set-Location $project.Directory.FullName
        dotnet build $project.FullName -c Release --framework net8.0
        Set-Location $PSScriptRoot
    }
    
    # Run tests with coverage reports
    dotnet test .\tests\AspNet.KickStarter.AddIn.AdditionalConfiguration.Tests\AspNet.KickStarter.AddIn.AdditionalConfiguration.Tests.csproj -c Release --no-build --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\tests\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/tests/TestResults/aiadc.xml /p:Include=[AspNet.KickStarter.AddIn.AdditionalConfiguration]* /p:Threshold=100
    dotnet test .\tests\AspNet.KickStarter.AddIn.ApplicationConfiguration.Tests\AspNet.KickStarter.AddIn.ApplicationConfiguration.Tests.csproj -c Release --no-build --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\tests\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/tests/TestResults/aiapc.xml /p:Include=[AspNet.KickStarter.AddIn.ApplicationConfiguration]* /p:Threshold=100
    dotnet test .\tests\AspNet.KickStarter.AddIn.Endpoints.Tests\AspNet.KickStarter.AddIn.Endpoints.Tests.csproj -c Release --no-build --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\tests\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/tests/TestResults/aie.xml /p:Include=[AspNet.KickStarter.AddIn.Endpoints]* /p:Threshold=100
    dotnet test .\tests\AspNet.KickStarter.AddIn.FluentValidation.Tests\AspNet.KickStarter.AddIn.FluentValidation.Tests.csproj -c Release --no-build --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\tests\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/tests/TestResults/aifv.xml /p:Include=[AspNet.KickStarter.AddIn.FluentValidation]* /p:Threshold=100
    dotnet test .\tests\AspNet.KickStarter.AddIn.HealthHandler.Tests\AspNet.KickStarter.AddIn.HealthHandler.Tests.csproj -c Release --no-build --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\tests\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/tests/TestResults/aihh.xml /p:Include=[AspNet.KickStarter.AddIn.HealthHandler]* /p:Threshold=100
    dotnet test .\tests\AspNet.KickStarter.AddIn.Mappings.Tests\AspNet.KickStarter.AddIn.Mappings.Tests.csproj -c Release --no-build --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\tests\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/tests/TestResults/aim.xml /p:Include=[AspNet.KickStarter.AddIn.Mappings]* /p:Threshold=100
    dotnet test .\tests\AspNet.KickStarter.AddIn.OpenTelemetry.Tests\AspNet.KickStarter.AddIn.OpenTelemetry.Tests.csproj -c Release --no-build --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\tests\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/tests/TestResults/aiot.xml /p:Include=[AspNet.KickStarter.AddIn.OpenTelemetry]* /p:Exclude=[*]*c__DisplayClass* /p:Threshold=100
    dotnet test .\tests\AspNet.KickStarter.AddIn.Serilog.Tests\AspNet.KickStarter.AddIn.Serilog.Tests.csproj -c Release --no-build --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\tests\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/tests/TestResults/aisl.xml /p:Include=[AspNet.KickStarter.AddIn.Serilog]* /p:Threshold=100
    dotnet test .\tests\AspNet.KickStarter.AddIn.Services.Tests\AspNet.KickStarter.AddIn.Services.Tests.csproj -c Release --no-build --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\tests\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/tests/TestResults/aisv.xml /p:Include=[AspNet.KickStarter.AddIn.Services]* /p:Threshold=100
    dotnet test .\tests\AspNet.KickStarter.AddIn.Swagger.Tests\AspNet.KickStarter.AddIn.Swagger.Tests.csproj -c Release --no-build --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\tests\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/tests/TestResults/aisw.xml /p:Include=[AspNet.KickStarter.AddIn.Swagger]* /p:Threshold=100
    dotnet test .\tests\AspNet.KickStarter.Core.Tests\AspNet.KickStarter.Core.Tests.csproj -c Release --no-build --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\tests\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/tests/TestResults/core.xml /p:Include=[AspNet.KickStarter.Core]* /p:Threshold=100
    dotnet test .\tests\AspNet.KickStarter.CQRS.Tests\AspNet.KickStarter.CQRS.Tests.csproj -c Release --no-build --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\tests\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/tests/TestResults/cqrs.xml /p:Include=[AspNet.KickStarter.CQRS]* /p:Threshold=100
    dotnet test .\tests\AspNet.KickStarter.FunctionalResult.Tests\AspNet.KickStarter.FunctionalResult.Tests.csproj -c Release --no-build --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\tests\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/tests/TestResults/func.xml /p:Include=[AspNet.KickStarter.FunctionalResult]* /p:Threshold=100
    dotnet test .\tests\AspNet.KickStarter.Tracing.Tests\AspNet.KickStarter.Tracing.Tests.csproj -c Release --no-build --framework net8.0 -l "console;verbosity=normal" --results-directory:"$PSScriptRoot\tests\TestResults" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=$PSScriptRoot/tests/TestResults/tr.xml /p:Include=[AspNet.KickStarter.Tracing]* /p:Threshold=100

    # Consolidate coverage reports
    reportgenerator -reports:'tests/TestResults/aiadc.xml;tests/TestResults/aiapc.xml;tests/TestResults/aie.xml;tests/TestResults/aifv.xml;tests/TestResults/aihh.xml;tests/TestResults/aim.xml;tests/TestResults/aiot.xml;tests/TestResults/aisl.xml;tests/TestResults/aisv.xml;tests/TestResults/aisw.xml;tests/TestResults/core.xml;tests/TestResults/core.xml;tests/TestResults/cqrs.xml;tests/TestResults/func.xml;tests/TestResults/tr.xml' -targetdir:tests/TestResults/CoverageReport -reporttypes:Html_Dark
    ./tests/TestResults/CoverageReport/index.html
}
finally {
    Set-Location $cwd
}