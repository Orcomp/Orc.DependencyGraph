@echo off
cls
if not exist tools\FAKE\tools\Fake.exe ( 
  tools\nuget\nuget.exe install FAKE -OutputDirectory tools -ExcludeVersion -Prerelease
)

REM TODO: tools\FAKE\tools\Fake.exe build.fsx %* "logfile=build-log.xml"
echo "TODO: Write FAKE script: build.fsx, and place it into the repo root
pause
