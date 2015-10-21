@ECHO OFF
@SETLOCAL

@SET SOLUTION=src\Serilog.Sinks.Kafka.sln
@SET MSBUILDARGS=/target:Rebuild /fileLogger /verbosity:minimal

@ECHO.
@ECHO  **** CLEAN  ****
MSBuild.exe %SOLUTION% /target:Clean /verbosity:minimal || GOTO BuildFailed

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