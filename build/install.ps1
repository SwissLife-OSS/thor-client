$runsOnAppVeyor = !!$env:APPVEYOR
$isRelease = $env:IS_RELEASE -eq "true"
$codeCoverageEnabled = $env:CODE_COVERAGE -eq "true"

if ($runsOnAppVeyor)
{
    choco install msbuild-sonarqube-runner -y

    $sonar = "SonarQube.Scanner.MSBuild.exe"

    if (!!$env:APPVEYOR_REPO_TAG_NAME) # Has a repo tag name
    {
        $env:CC_BUILD_VERSION = $env:APPVEYOR_REPO_TAG_NAME
    }
    else
    {
        $env:CC_BUILD_VERSION = $env:APPVEYOR_BUILD_VERSION
    }
}