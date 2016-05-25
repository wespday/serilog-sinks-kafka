@ECHO OFF
@SETLOCAL

@ECHO.
@ECHO  **** STARTING BUILD  ****

CALL "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsDevCmd.bat"

@SET THIS_SCRIPT_FOLDER=%~dp0
CD "%THIS_SCRIPT_FOLDER%"
@SET ARTIFACTS=artifacts
@SET NUGET_COMMAND=src\.nuget\nuget.exe
@SET PACKAGE_VERSION=%APPVEYOR_BUILD_VERSION%

IF "%PACKAGE_VERSION%"=="" (
	SET PACKAGE_VERSION=0.0.1
)

SET PRERELEASE_PACKAGE_VERSION=%PACKAGE_VERSION%-prerelease

@SET NUGET_PACKAGE_ID=Serilog.Sinks.Kafka
@SET SOLUTION=%THIS_SCRIPT_FOLDER%src\Serilog.Sinks.Kafka.sln
@SET PROJECT_FOLDER=src\%NUGET_PACKAGE_ID%
@SET MSBUILDARGS=/target:Rebuild /fileLogger /verbosity:minimal /ToolsVersion:14.0
@SET NUGET_PACKAGE_FOLDER=%ARTIFACTS%\%NUGET_PACKAGE_ID%
@SET DOTNET_FRAMEWORK=net45
@SET NUGET_FRAMEWORK_FOLDER=%NUGET_PACKAGE_FOLDER%\lib\%DOTNET_FRAMEWORK%

RMDIR /Q /S "%ARTIFACTS%" >nul 2>&1
MKDIR "%ARTIFACTS%"

@ECHO.
@ECHO  **** RESTORE NUGET PACKAGES  ****
"%NUGET_COMMAND%" restore "%SOLUTION%"  -Verbosity quiet ||  GOTO BuildFailed

@ECHO.
@ECHO  **** BUIILD DEBUG  ****
MSBuild "%SOLUTION%" %MSBUILDARGS% ||  GOTO BuildFailed
COPY msbuild.log "%ARTIFACTS%\msbuild.DEBUG.log" ||  GOTO BuildFailed

IF "%APPVEYOR_BUILD_VERSION%"=="" (
	@ECHO.
	@ECHO  **** UNIT TEST ****
	mstest /nologo /category:unit ^
	/testcontainer:src\Serilog.Sinks.Kafka.Test\bin\Debug\Serilog.Sinks.Kafka.Test.dll
)

@ECHO.
@ECHO  **** BUIILD RELEASE  ****
MSBuild "%SOLUTION%" %MSBUILDARGS% /property:Configuration=Release ||  GOTO BuildFailed
COPY msbuild.log "%ARTIFACTS%\msbuild.RELEASE.log" ||  GOTO BuildFailed

@ECHO.
@ECHO  **** STAGE NUGET PACKAGE FOLDER ****
MKDIR "%NUGET_FRAMEWORK_FOLDER%" ||  GOTO BuildFailed
COPY "%PROJECT_FOLDER%\bin\Release\%NUGET_PACKAGE_ID%.??l" "%NUGET_FRAMEWORK_FOLDER%\" ||  GOTO BuildFailed
COPY "%PROJECT_FOLDER%\bin\Release\%NUGET_PACKAGE_ID%.pdb" "%NUGET_FRAMEWORK_FOLDER%\" ||  GOTO BuildFailed
MKDIR "%NUGET_FRAMEWORK_FOLDER%\CodeContracts\" ||  GOTO BuildFailed
COPY "%PROJECT_FOLDER%\bin\Release\CodeContracts\*.*" "%NUGET_FRAMEWORK_FOLDER%\CodeContracts\" ||  GOTO BuildFailed
COPY "%PROJECT_FOLDER%\bin\Release\%NUGET_PACKAGE_ID%.nuspec" "%NUGET_PACKAGE_FOLDER%\" ||  GOTO BuildFailed

@ECHO.
@ECHO  **** CREATE PRE-RELEASE NUGET PACKAGE ****
%NUGET_COMMAND% pack "%NUGET_PACKAGE_FOLDER%\%NUGET_PACKAGE_ID%.nuspec" -Version %PRERELEASE_PACKAGE_VERSION% ^
     -NonInteractive -OutputDirectory %ARTIFACTS% -Properties version=%PACKAGE_VERSION%||  GOTO BuildFailed

@ECHO.
@ECHO  **** CREATE RELEASE NUGET PACKAGE ****
%NUGET_COMMAND% pack "%NUGET_PACKAGE_FOLDER%\%NUGET_PACKAGE_ID%.nuspec" -Version %PACKAGE_VERSION% ^
     -NonInteractive -OutputDirectory %ARTIFACTS% -Properties version=%PACKAGE_VERSION%||  GOTO BuildFailed

 
@ECHO.
@ECHO **** BUILD SUCCESSFUL ****
GOTO:EOF

:BuildFailed
@ECHO.
@ECHO *** BUILD FAILED ***
EXIT /B -1