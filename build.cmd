@ECHO OFF
@SETLOCAL

@ECHO.
@ECHO  **** STARTING BUILD  ****

@SET SRC=%~dp0\src
@SET SOLUTION=%SRC%\Serilog.Sinks.Kafka.sln
@SET MSBUILDARGS=/target:Rebuild /fileLogger /verbosity:minimal
@SET NUGET_COMMAND=%SRC%\.nuget\nuget.exe
@SET Month=%%A
@SET DAY=%%B
@SET Year=%%C

@ECHO.
@ECHO  **** CLEAN  ****
MSBuild.exe %SOLUTION% /target:Clean /verbosity:minimal || GOTO BuildFailed

%NUGET_COMMAND% restore "%SOLUTION%"  -Verbosity quiet ||  GOTO BuildFailed

@ECHO.
@ECHO  **** BUIILD DEBUG  ****
MSBuild.exe %SOLUTION%  %MSBUILDARGS% /property:Configuration=Debug %* || GOTO BuildFailed

@ECHO.
@ECHO  **** BUIILD RELEASE  ****
MSBuild.exe %SOLUTION% %MSBUILDARGS% /property:Configuration=Release %* || GOTO BuildFailed

@ECHO.
@ECHO **** BUILD SUCCESSFUL ****

@ECHO **** BUILDING Nuget ****
@SET version=%DATE:~10,4%%DATE:~4,2%%DATE:~7,2%%TIME:~0,2%

echo %version%
%NUGET_COMMAND% pack %SRC%\Serilog.Sinks.Kafka\Serilog.Sinks.Kafka.csproj -Build -Symbols -Properties Configuration=Release -Version 1.0.%version% -Verbosity quiet
%NUGET_COMMAND% push *.nupkg %NUGET_REPOSITORY_API_KEY% -source http://nuget.ual.com/packages
GOTO:EOF

:BuildFailed
@ECHO.
@ECHO *** BUILD FAILED ***
EXIT /B -1