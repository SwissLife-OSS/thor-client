if (Test-Path env:APPVEYOR)
{
    # Restore packages
    dotnet restore .\src\Tracing.sln

    # Build & Analyse
    choco install "msbuild-sonarqube-runner" -y
    Invoke-Expression ('SonarQube.Scanner.MSBuild.exe begin /n:"' + $env:APPVEYOR_PROJECT_NAME + '" /k:"' + $env:SONARQUBE_PROJECT_KEY + '" /d:"sonar.host.url=https://sonarqube.com" /d:"sonar.login=' + $env:SONARQUBE_TOKEN + '" /v:"' + $env:CC_BUILD_VERSION + '"')
    msbuild .\src\Tracing.sln /p:Configuration=$env:CONFIGURATION /p:Version=$env:CC_BUILD_VERSION
    Invoke-Expression ('SonarQube.Scanner.MSBuild.exe end /d:"sonar.login=' + $env:SONARQUBE_TOKEN + '"')
}