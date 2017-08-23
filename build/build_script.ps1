# Restore packages
dotnet restore .\src\Tracing.sln

# Build & Analyse
# Sonar for .Net Core is not supported right now; for more information go to https://jira.sonarsource.com/browse/SONARMSBRU-310
#Invoke-Expression ('SonarQube.Scanner.MSBuild.exe begin /k:"' + $env:APPVEYOR_PROJECT_NAME + '" /d:"sonar.host.url=https://sonarqube.com" /d:"sonar.login=' + $env:SONARQUBE_TOKEN + '" /v:"' + $env:APPVEYOR_REPO_TAG_NAME + '"')
dotnet build .\src\Tracing.sln -c $env:CONFIGURATION /p:Version=$env:APPVEYOR_REPO_TAG_NAME
#Invoke-Expression ('SonarQube.Scanner.MSBuild.exe end /d:"sonar.login=' + $env:SONARQUBE_TOKEN + '"')