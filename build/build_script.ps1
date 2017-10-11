if ($runsOnAppVeyor)
{
    # Restore packages
    dotnet restore .\src\Tracing.sln

    if ($isRelease)
    {
        # Start Analyzation
        # Sonar for .Net Core is not supported right now; for more information go to https://jira.sonarsource.com/browse/SONARMSBRU-310
        & $sonar ' begin /n:"' + $env:APPVEYOR_PROJECT_NAME + '" /k:"' + $env:SONAR_PROJECT_KEY + '" /v:"' + $env:CC_BUILD_VERSION + '" /d:"sonar.host.url=https://sonarcloud.io" /d:"sonar.login=' + $env:SONAR_TOKEN + '" /d:"sonar.organization=' + $env:SONAR_ORGANIZATION_KEY + '"'
    }

    # Build 
    msbuild .\src\Tracing.sln /p:Configuration=Debug /p:Version=$env:CC_BUILD_VERSION

    # Test
    $serachDirs = ".\src\*\bin\Debug\netcoreapp2.0"
    $testAssemblies = $null
    
    Get-ChildItem -Path $serachDirs -Include *.Tests.dll -Recurse | %{ $testAssemblies += $_.FullName + " " }

    if (!!$testAssemblies) # Has test assemblies
    {
        $vstest = "vstest.console.exe"
        $vstestArguments = $testAssemblies + "/Framework:FrameworkCore10"
    
        if ($runsOnAppVeyor)
        {
            $vstestArguments += " /logger:Appveyor"
        }

        if ($codeCoverageEnabled)
        {
            # Test & Code Coverage
            $openCover = Join-Path -Path $env:USERPROFILE -ChildPath ".nuget\packages\OpenCover\*\tools\OpenCover.Console.exe" -Resolve
            $coveralls = Join-Path -Path $env:USERPROFILE -ChildPath ".nuget\packages\coveralls.io\*\tools\coveralls.net.exe" -Resolve

            & $openCover ' -register:user -target:"' + $vstest + '" -targetargs: "' + $vstestArguments + '" -searchdirs:"' + $serachDirs + '" -oldstyle -output:coverage.xml -skipautoprops -returntargetcode -filter:"+[*Tracing]*"'
            & $coveralls ' --opencover coverage.xml'
        }
        else
        {
            # Test
            & $vstest $vstestArguments
        }
    }

    if ($isRelease)
    {
        # End Analyzation
        & $sonar ' end /d:"sonar.login=' + $env:SONAR_TOKEN + '"'

        # Pack
        dotnet pack .\src\Tracing.sln --include-symbols --include-source -c Release /p:PackageVersion=$env:CC_BUILD_VERSION
    }
}