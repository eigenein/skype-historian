# Skype Historian Installer Build Script
!include "MUI.nsh"

Name "Skype Historian"
OutFile "..\Installers\Skype Historian Setup.exe"
InstallDir "$LOCALAPPDATA\Skype Historian"
RequestExecutionLevel user

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

Section

SetOutPath $INSTDIR
WriteUninstaller $INSTDIR\Uninstaller.exe

SectionEnd

Section "Uninstall"

Delete $INSTDIR\Uninstaller.exe

SectionEnd
