echo off

rem Create temporary folder
set temp_folder=%TEMP%\build_temp
md %temp_folder%

rem Copy files to temporary folder
copy bin\Debug\net3.5\SaveStates.dll %temp_folder%
copy Info.json %temp_folder%
copy README.md %temp_folder%

rem Zip folder
powershell Compress-Archive -Update -Path %temp_folder%\* -DestinationPath PhoA-SaveStates.zip

rem Delete temporary folder
rmdir /s /q %temp_folder%