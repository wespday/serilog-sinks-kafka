@ECHO OFF
@SETLOCAL

@ECHO.
@ECHO  **** STARTING BUILD  ****

@SET SRC=%~dp0\src
@SET SOLUTION=%SRC%\Serilog.Sinks.Kafka.sln
@SET MSBUILDARGS=/target:Rebuild /fileLogger /verbosity:minimal
@SET NUGET_COMMAND=%SRC%\.nuget\nuget.exe

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
GOTO:EOF

:BuildFailed
@ECHO.
@ECHO *** BUILD FAILED ***
EXIT /B -1