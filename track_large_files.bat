@echo off
setlocal EnableDelayedExpansion

REM 设置文件大小限制（以字节为单位）
set SIZE_LIMIT=104857600  REM 100MB

REM 查找所有大于指定大小的文件并跟踪
set "large_files="

REM 在当前目录查找大文件
for /r %%f in (*) do (
    REM 获取文件大小
    set "size=%%~zf"
    if !size! gtr !SIZE_LIMIT! (
        REM 输出找到的大文件
        echo Tracking large file: %%f
        
        REM 使用 git lfs track 命令跟踪该文件
        git lfs track "%%f"
        
        REM 将大文件添加到数组
        set "large_files=!large_files! %%f"
    )
)

REM 提交更改
if defined large_files (
    REM 添加 .gitattributes 文件
    git add .gitattributes

    REM 添加大文件
    for %%f in (!large_files!) do (
        git add "%%f"
    )

    REM 提交更改
    git commit -m "Automatically track large files with Git LFS"
    
    echo Tracked !large_files! large files and updated .gitattributes.
) else (
    echo No large files found to track.
)

endlocal
pause
