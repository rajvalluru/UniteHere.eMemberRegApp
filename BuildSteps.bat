REM Run the below commands in command window
@echo off
call %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe /p:DeployOnBuild=true /p:Configuration=Release UniteHere.ElectionApp\UniteHere.ElectionApp.sln
call %windir%\Microsoft.NET\Framework\v4.0.30319\aspnet_compiler.exe -v / -p UniteHere.ElectionApp\UniteHere.ElectionApp\obj\Release\Package\PackageTmp -c ..\deployment\iis-package\ElectionApp
if not exist "..\deployment\iis-package\ElectionApp\Content\Report_OutPut" mkdir ..\deployment\iis-package\ElectionApp\Content\Report_OutPut
@echo on
echo done
