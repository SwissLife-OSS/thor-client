if (Test-Path env:APPVEYOR)
{
    if (Test-Path env:PRBUILD)
    {
        .\build\runtests.bat
    }
    else
    {
        Invoke-Expression ((Get-ChildItem ($env:USERPROFILE + '\.nuget\packages\OpenCover'))[0].FullName + '\tools\OpenCover.Console.exe -register:user -target:"build\runtests.bat" -searchdirs:".\src\*\bin\Debug\netstandard2.0" -oldstyle -output:coverage.xml -skipautoprops -returntargetcode -filter:"+[*Tracing]*"')
        Invoke-Expression ((Get-ChildItem ($env:USERPROFILE + '\.nuget\packages\coveralls.io'))[0].FullName + '\tools\coveralls.net.exe --opencover coverage.xml')
        dotnet pack .\src\Tracing.sln --include-symbols --include-source -c $env:CONFIGURATION /p:PackageVersion=$env:CC_BUILD_VERSION
    }
}