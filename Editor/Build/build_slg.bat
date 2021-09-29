set svn_path=C:/work
set project_path=C:/work/teama/slg
set debug=%1%
set up_version=%2%
set platform=%3%
set use_hotfix=%4%
set use_aab=%5%
REM `C:/Program" "Files/Unity/Hub/Editor/2019.4.24f1c1/Editor/Unity.exe -batchmode -quit -executeMethod Framework.Editor.Build.JenkinsBuild -logFile $build_path/buildLog.log -projectPath $project_path --BUILDPATH:$build_path --DEBUG:$1 --UPVERSION:$2 --PLATFORM:$3 --USEHOTFIX:$4 --AAB:$5`
REM `C:/Program" "Files/Unity/Hub/Editor/2019.4.24f1c1/Editor/Unity.exe -batchmode -quit -executeMethod Framework.Editor.Build.JenkinsBuild -logFile $build_path/buildLog.log -projectPath $project_path --BUILDPATH:$build_path --DEBUG:true --UPVERSION:false --PLATFORM:StandaloneWindows --USEHOTFIX:false --AAB:false`
python C:/Users/BJ-2101/Desktop/jenkins_build/build.py %project_path% %debug% %up_version% %platform% %use_hotfix% %use_aab% %svn_path% %qq_group%
REM python C:/Users/BJ-2101/Desktop/jenkins_build/build.py %project_path% true false StandaloneWindows false false %svn_path% %qq_group%
echo build finish
IF %ERRORLEVEL% NEQ 0 GOTO ProcessError
exit /b 0    
:ProcessError
exit /b 1
pause