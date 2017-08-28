if (Test-Path env:APPVEYOR)
{
    # Restore packages
    dotnet restore .\src\Tracing.sln

    # Build & Analyse
    # Sonar for .Net Core is not supported right now; for more information go to https://jira.sonarsource.com/browse/SONARMSBRU-310
    #Invoke-Expression ('SonarQube.Scanner.MSBuild.exe begin /k:"' + $env:APPVEYOR_PROJECT_NAME + '" /d:"sonar.host.url=https://sonarqube.com" /d:"sonar.login=' + $env:SONARQUBE_TOKEN + '" /v:"' + $env:CC_BUILD_VERSION + '"')
    dotnet build .\src\Tracing.sln -c $env:CONFIGURATION /p:Version=$env:CC_BUILD_VERSION
    #Invoke-Expression ('SonarQube.Scanner.MSBuild.exe end /d:"sonar.login=' + $env:SONARQUBE_TOKEN + '"')
}