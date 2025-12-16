@echo off
REM ========================================
REM MOTHER-BRAIN One-Click Windows Setup
REM Fully automated - just double-click!
REM ========================================

echo.
echo ================================================
echo  MOTHER-BRAIN Auto-Setup for Windows
echo ================================================
echo.
echo This will automatically:
echo  - Install OpenCode SST
echo  - Find your Drive keyz folder
echo  - Configure all MCP servers
echo  - Set up environment variables
echo.
echo No manual steps required!
echo.
pause

REM Change to user home directory
cd /d %USERPROFILE%

REM ========================================
REM Step 1: Install Node.js if missing
REM ========================================
echo.
echo [1/6] Checking Node.js...
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo Node.js not found! Downloading...
    echo Please install Node.js from: https://nodejs.org/
    start https://nodejs.org/en/download/
    echo.
    echo After installing Node.js, run this script again.
    pause
    exit /b 1
)
echo Node.js found!

REM ========================================
REM Step 2: Install OpenCode SST
REM ========================================
echo.
echo [2/6] Installing OpenCode SST...
call npm install -g opencode-ai@latest
if %errorlevel% neq 0 (
    echo Failed to install OpenCode. Trying with admin rights...
    echo Please run this script as Administrator if it fails.
)
echo OpenCode installed!

REM ========================================
REM Step 3: Find keyz folder automatically
REM ========================================
echo.
echo [3/6] Searching for keyz folder...

REM Check common Drive locations
set "KEYZ_FOLDER="
if exist "%USERPROFILE%\Google Drive\keyz" set "KEYZ_FOLDER=%USERPROFILE%\Google Drive\keyz"
if exist "%USERPROFILE%\GoogleDrive\keyz" set "KEYZ_FOLDER=%USERPROFILE%\GoogleDrive\keyz"
if exist "G:\keyz" set "KEYZ_FOLDER=G:\keyz"
if exist "C:\Users\%USERNAME%\Google Drive\keyz" set "KEYZ_FOLDER=C:\Users\%USERNAME%\Google Drive\keyz"
if exist "%USERPROFILE%\Drive\keyz" set "KEYZ_FOLDER=%USERPROFILE%\Drive\keyz"
if exist "%USERPROFILE%\wormhole\keyz" set "KEYZ_FOLDER=%USERPROFILE%\wormhole\keyz"

REM Check the shared Drive folder from your link
if exist "%USERPROFILE%\Google Drive\My Drive\keyz" set "KEYZ_FOLDER=%USERPROFILE%\Google Drive\My Drive\keyz"

if "%KEYZ_FOLDER%"=="" (
    echo Could not find keyz folder automatically.
    echo.
    set /p "KEYZ_FOLDER=Enter full path to your keyz folder: "
)

if not exist "%KEYZ_FOLDER%" (
    echo ERROR: keyz folder not found at: %KEYZ_FOLDER%
    echo.
    echo Please make sure Google Drive is synced.
    pause
    exit /b 1
)

echo Found keyz folder: %KEYZ_FOLDER%

REM ========================================
REM Step 4: Create OpenCode MCP Config
REM ========================================
echo.
echo [4/6] Creating OpenCode MCP configuration...

REM Create config directory
if not exist "%USERPROFILE%\.config\opencode" mkdir "%USERPROFILE%\.config\opencode"

REM Create opencode.json with escaped paths
echo { > "%USERPROFILE%\.config\opencode\opencode.json"
echo   "mcpServers": { >> "%USERPROFILE%\.config\opencode\opencode.json"
echo     "filesystem": { >> "%USERPROFILE%\.config\opencode\opencode.json"
echo       "command": "npx", >> "%USERPROFILE%\.config\opencode\opencode.json"
echo       "args": ["-y", "@modelcontextprotocol/server-filesystem"], >> "%USERPROFILE%\.config\opencode\opencode.json"
echo       "env": { >> "%USERPROFILE%\.config\opencode\opencode.json"
echo         "ALLOWED_DIRECTORIES": "C:\\wormhole;D:\\wormhole;C:\\workspace\\wormhole\\MOTHER_BRAIN\\temporal_raw_data;%USERPROFILE:\=\\%\\Documents;%USERPROFILE:\=\\%\\Downloads;%KEYZ_FOLDER:\=\\%" >> "%USERPROFILE%\.config\opencode\opencode.json"
echo       } >> "%USERPROFILE%\.config\opencode\opencode.json"
echo     }, >> "%USERPROFILE%\.config\opencode\opencode.json"
echo     "github": { >> "%USERPROFILE%\.config\opencode\opencode.json"
echo       "command": "npx", >> "%USERPROFILE%\.config\opencode\opencode.json"
echo       "args": ["-y", "@modelcontextprotocol/server-github"], >> "%USERPROFILE%\.config\opencode\opencode.json"
echo       "env": { >> "%USERPROFILE%\.config\opencode\opencode.json"
echo         "GITHUB_PERSONAL_ACCESS_TOKEN": "${GITHUB_TOKEN}" >> "%USERPROFILE%\.config\opencode\opencode.json"
echo       } >> "%USERPROFILE%\.config\opencode\opencode.json"
echo     }, >> "%USERPROFILE%\.config\opencode\opencode.json"
echo     "gdrive": { >> "%USERPROFILE%\.config\opencode\opencode.json"
echo       "command": "npx", >> "%USERPROFILE%\.config\opencode\opencode.json"
echo       "args": ["-y", "@modelcontextprotocol/server-gdrive"], >> "%USERPROFILE%\.config\opencode\opencode.json"
echo       "env": { >> "%USERPROFILE%\.config\opencode\opencode.json"
echo         "GOOGLE_APPLICATION_CREDENTIALS": "%KEYZ_FOLDER:\=\\%\\gdrive-sa.json" >> "%USERPROFILE%\.config\opencode\opencode.json"
echo       } >> "%USERPROFILE%\.config\opencode\opencode.json"
echo     }, >> "%USERPROFILE%\.config\opencode\opencode.json"
echo     "puppeteer": { >> "%USERPROFILE%\.config\opencode\opencode.json"
echo       "command": "npx", >> "%USERPROFILE%\.config\opencode\opencode.json"
echo       "args": ["-y", "@modelcontextprotocol/server-puppeteer"] >> "%USERPROFILE%\.config\opencode\opencode.json"
echo     } >> "%USERPROFILE%\.config\opencode\opencode.json"
echo   } >> "%USERPROFILE%\.config\opencode\opencode.json"
echo } >> "%USERPROFILE%\.config\opencode\opencode.json"

echo OpenCode MCP config created!

REM ========================================
REM Step 5: Create Environment File
REM ========================================
echo.
echo [5/6] Creating environment configuration...

echo # MOTHER-BRAIN Environment Variables > "%USERPROFILE%\.mother-brain.env"
echo # Auto-generated on %date% %time% >> "%USERPROFILE%\.mother-brain.env"
echo. >> "%USERPROFILE%\.mother-brain.env"
echo KEYZ_FOLDER=%KEYZ_FOLDER% >> "%USERPROFILE%\.mother-brain.env"
echo GDRIVE_ROOT_FOLDER_ID=1VSH08EzxY0Knni5HKUaHePTliUAKxfWh >> "%USERPROFILE%\.mother-brain.env"
echo. >> "%USERPROFILE%\.mother-brain.env"
echo # API keys will be loaded from keyz folder >> "%USERPROFILE%\.mother-brain.env"

echo Environment file created!

REM ========================================
REM Step 6: Create Quick Launch Script
REM ========================================
echo.
echo [6/6] Creating quick-launch shortcuts...

REM Create a startup script
echo @echo off > "%USERPROFILE%\Start-MOTHER-BRAIN.bat"
echo echo. >> "%USERPROFILE%\Start-MOTHER-BRAIN.bat"
echo echo ======================================== >> "%USERPROFILE%\Start-MOTHER-BRAIN.bat"
echo echo  MOTHER-BRAIN Environment Ready! >> "%USERPROFILE%\Start-MOTHER-BRAIN.bat"
echo echo ======================================== >> "%USERPROFILE%\Start-MOTHER-BRAIN.bat"
echo echo. >> "%USERPROFILE%\Start-MOTHER-BRAIN.bat"
echo echo Keyz folder: %KEYZ_FOLDER% >> "%USERPROFILE%\Start-MOTHER-BRAIN.bat"
echo echo OpenCode config: %USERPROFILE%\.config\opencode\opencode.json >> "%USERPROFILE%\Start-MOTHER-BRAIN.bat"
echo echo. >> "%USERPROFILE%\Start-MOTHER-BRAIN.bat"
echo echo Starting OpenCode... >> "%USERPROFILE%\Start-MOTHER-BRAIN.bat"
echo echo. >> "%USERPROFILE%\Start-MOTHER-BRAIN.bat"
echo opencode >> "%USERPROFILE%\Start-MOTHER-BRAIN.bat"

REM ========================================
REM Setup Complete!
REM ========================================
echo.
echo.
echo ================================================
echo  SETUP COMPLETE!
echo ================================================
echo.
echo Your system is now configured with:
echo.
echo  OpenCode SST:     Installed globally
echo  MCP Config:       %USERPROFILE%\.config\opencode\opencode.json
echo  Keyz Folder:      %KEYZ_FOLDER%
echo  Environment:      %USERPROFILE%\.mother-brain.env
echo.
echo ================================================
echo  NEXT STEPS (if API keys not in keyz folder):
echo ================================================
echo.
echo Place these files in: %KEYZ_FOLDER%
echo.
echo   - github_token.txt        (your GitHub token)
echo   - gemini_api_key.txt      (your Gemini key)
echo   - perplexity_api_key.txt  (your Perplexity key)
echo   - openai_api_key.txt      (your OpenAI key)
echo   - gdrive-sa.json          (Google service account)
echo.
echo ================================================
echo  TO START OPENCODE:
echo ================================================
echo.
echo Option 1: Double-click: %USERPROFILE%\Start-MOTHER-BRAIN.bat
echo Option 2: Run: opencode
echo Option 3: Run: cd %USERPROFILE%\MOTHER-BRAIN ^&^& opencode
echo.
echo Your wormhole automation is READY!
echo.
pause

REM Open the keyz folder so you can verify/add keys
explorer "%KEYZ_FOLDER%"
