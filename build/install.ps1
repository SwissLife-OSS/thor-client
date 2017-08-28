if (Test-Path env:APPVEYOR)
{
    # Sonar for .Net Core is not supported right now; for more information go to https://jira.sonarsource.com/browse/SONARMSBRU-310
    #choco install msbuild-sonarqube-runner -y

    if (Test-Path env:APPVEYOR_REPO_TAG_NAME)
    {
        $env:CC_BUILD_VERSION = $env:APPVEYOR_REPO_TAG_NAME
    }
    else
    {
        $env:CC_BUILD_VERSION = $env:APPVEYOR_BUILD_VERSION
    }
}