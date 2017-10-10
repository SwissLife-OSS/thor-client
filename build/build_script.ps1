if (Test-Path env:APPVEYOR)
{
    # Restore packages
    dotnet restore .\src\Tracing.sln

    # Build & Analyse
    Invoke-Expression ('SonarQube.Scanner.MSBuild.exe begin /n:"' + $env:APPVEYOR_PROJECT_NAME + '" /k:"' + $env:SONAR_PROJECT_KEY + '" /v:"' + $env:CC_BUILD_VERSION + '" /d:"sonar.host.url=https://sonarcloud.io" /d:"sonar.login=' + $env:SONAR_TOKEN + '" /d:"sonar.organization=' + $env:SONAR_ORGANIZATION_KEY + '"')
    msbuild .\src\Tracing.sln /p:Configuration=$env:CONFIGURATION /p:Version=$env:CC_BUILD_VERSION
    Invoke-Expression ('SonarQube.Scanner.MSBuild.exe end /d:"sonar.login=' + $env:SONAR_TOKEN + '"')
}