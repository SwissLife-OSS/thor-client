$runsOnAppVeyor = !!$env:APPVEYOR
$isRelease = $env:IS_RELEASE -eq "true"
$codeCoverageEnabled = $env:CODE_COVERAGE -eq "true"

Write-Host "Runs on AppVayor:" $runsOnAppVeyor
Write-Host "Is Release build:" $isRelease
Write-Host "Is Code Coverage enabled:" $codeCoverageEnabled

if ($runsOnAppVeyor)
{
    # Install Analyzer
    if ($isRelease)
    {
        choco install msbuild-sonarqube-runner -y

        $sonar = "SonarQube.Scanner.MSBuild.exe"
    }

    # Set version
    if (!!$env:APPVEYOR_REPO_TAG_NAME) # Has a repo tag name
    {
        $env:CC_BUILD_VERSION = $env:APPVEYOR_REPO_TAG_NAME
    }
    else
    {
        $env:CC_BUILD_VERSION = $env:APPVEYOR_BUILD_VERSION
    }

    # Restore packages
    dotnet restore .\src\Tracing.sln

    if ($isRelease)
    {
        # Start Analyzation
        # Sonar for .Net Core is not supported right now; for more information go to https://jira.sonarsource.com/browse/SONARMSBRU-310
        Invoke-Expression ($sonar + ' begin /n:"' + $env:APPVEYOR_PROJECT_NAME + '" /k:"' + $env:SONAR_PROJECT_KEY + '" /v:"' + $env:CC_BUILD_VERSION + '" /d:"sonar.host.url=https://sonarcloud.io" /d:"sonar.login=' + $env:SONAR_TOKEN + '" /d:"sonar.organization=' + $env:SONAR_ORGANIZATION_KEY + '"')
    }

    # Build 
    msbuild .\src\Tracing.sln /p:Configuration=Debug /p:Version=$env:CC_BUILD_VERSION

    # Test
    $serachDirs = ".\src\*\bin\Debug\netcoreapp2.0"
    $testAssemblies = $null
    
    Get-ChildItem -Path $serachDirs -Include *.Tests.dll -Recurse | %{ $testAssemblies += $_.FullName + " " }

    if (!!$testAssemblies) # Has test assemblies
    {
        $vstest = Join-Path -Path "C:\*\Microsoft Visual Studio\2017\*\Common7\IDE\Extensions\TestPlatform" -ChildPath "vstest.console.exe" -Resolve
        $vstestFramework = "/Framework:FrameworkCore10"
        $vstestLogger = $null
    
        if ($runsOnAppVeyor)
        {
            $vstestLogger = "/logger:Appveyor"
        }

        if ($codeCoverageEnabled)
        {
            # Test & Code Coverage
            $openCover = Join-Path -Path $env:USERPROFILE -ChildPath ".nuget\packages\OpenCover\*\tools\OpenCover.Console.exe" -Resolve
            $coveralls = Join-Path -Path $env:USERPROFILE -ChildPath ".nuget\packages\coveralls.io\*\tools\coveralls.net.exe" -Resolve

            Invoke-Expression ($openCover + ' -register:user -target:"' + $vstest + '" -targetargs:"' + $testAssemblies + ' ' + $vstestFramework + ' ' + $vstestLogger + '" -searchdirs:"' + $serachDirs + '" -oldstyle -output:coverage.xml -skipautoprops -returntargetcode -filter:"+[*Tracing]*"')
            Invoke-Expression ($coveralls + ' --opencover coverage.xml')
        }
        else
        {
            # Test
            Invoke-Expression ($vstest + ' ' + $testAssemblies + ' ' + $vstestFramework + ' ' + $vstestLogger)
        }
    }

    if ($isRelease)
    {
        # End Analyzation
        Invoke-Expression ($sonar + ' end /d:"sonar.login=' + $env:SONAR_TOKEN + '"')

        # Pack
        dotnet pack .\src\Tracing.sln --include-symbols --include-source -c Release /p:PackageVersion=$env:CC_BUILD_VERSION
    }
}