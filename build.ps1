param([switch]$DisableBuild, [switch]$RunTests, [switch]$EnableCoverage, [switch]$EnableSonar, [switch]$Pack)

if (!!$env:APPVEYOR_REPO_TAG_NAME)
{
    $version = $env:APPVEYOR_REPO_TAG_NAME
}
elseif(!!$env:APPVEYOR_BUILD_VERSION)
{
    $version = $env:APPVEYOR_BUILD_VERSION
}

if($version -ne $null)
{
    $env:Version = $version
}

if($EnableSonar)
{
    # Sonar for .Net Core is not supported right now; for more information go to https://jira.sonarsource.com/browse/SONARMSBRU-310
    #choco install msbuild-sonarqube-runner -y
    #Invoke-Expression ('SonarQube.Scanner.MSBuild.exe begin /k:"' + $env:APPVEYOR_PROJECT_NAME + '" /d:"sonar.host.url=https://sonarqube.com" /d:"sonar.login=' + $env:SONARQUBE_TOKEN + '" /v:"' + $env:CC_BUILD_VERSION + '"')
}

if($DisableBuild -eq $false)
{
    dotnet restore src
    msbuild src
}

if($RunTests -or $EnableCoverage)
{
    # Test
    $serachDirs = [System.IO.Path]::Combine($PSScriptRoot, "src", "*", "bin",  "Debug", "netcoreapp2.0")
    $runTestsCmd = [System.Guid]::NewGuid().ToString("N") + ".cmd"
    $runTestsCmd = Join-Path -Path $env:TEMP -ChildPath $runTestsCmd
    $testAssemblies = ""
    
    Get-ChildItem ./src/*.Tests | %{ $testAssemblies += "dotnet test `"" + $_.FullName + "`" --no-build`n" }
    
    if (!!$testAssemblies) # Has test assemblies
    {    
        $userDirectory = $env:USERPROFILE
        if($IsMacOS) 
        {
            $userDirectory = $env:HOME
        }
        
        [System.IO.File]::WriteAllText($runTestsCmd, $testAssemblies)
        Write-Host $runTestsCmd

        if ($EnableCoverage)
        {
            # Test & Code Coverage
            $nugetPackages = [System.IO.Path]::Combine($userDirectory, ".nuget", "packages")
            
            $openCover = [System.IO.Path]::Combine($nugetPackages, "OpenCover", "*", "tools",  "OpenCover.Console.exe")
            $openCover = Resolve-Path $openCover

            $coveralls = [System.IO.Path]::Combine($nugetPackages, "coveralls.io", "*", "tools",  "coveralls.net.exe")
            $coveralls = Resolve-Path $coveralls

            & $openCover -register:user -target:"$runTestsCmd" -searchdirs:"$serachDirs" -oldstyle -output:coverage.xml -skipautoprops -returntargetcode -filter:"+[Thor*]*"
            & $coveralls --opencover coverage.xml
        }
        else
        {
            # Test
            & $runTestsCmd
        }
    }
}

if($EnableSonar)
{
    # Sonar for .Net Core is not supported right now; for more information go to https://jira.sonarsource.com/browse/SONARMSBRU-310
    #Invoke-Expression ('SonarQube.Scanner.MSBuild.exe end /d:"sonar.login=' + $env:SONARQUBE_TOKEN + '"')
}

if($Pack)
{
    dotnet pack src --include-symbols --include-source -c Release /p:PackageVersion=$version
}