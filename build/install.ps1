if (Test-Path env:APPVEYOR)
{
    choco install msbuild-sonarqube-runner -y

    if (Test-Path env:APPVEYOR_REPO_TAG_NAME)
    {
        $env:CC_BUILD_VERSION = $env:APPVEYOR_REPO_TAG_NAME
    }
    else
    {
        $env:CC_BUILD_VERSION = $env:APPVEYOR_BUILD_VERSION
    }
}