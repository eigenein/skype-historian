; Skype Historian Installer Build Script

; Include Modern UI

    !include "MUI.nsh"

; General

    Name "Skype Historian"
    OutFile "..\Installers\Skype Historian Setup.exe"
    
    ; Default installation folder
    InstallDir "$LOCALAPPDATA\Skype Historian"
    
    ; Request application priveledges for Vista/7
    RequestExecutionLevel user

; Interface Settings

  !define MUI_ABORTWARNING

  ; Show all languages, despite user's codepage
  !define MUI_LANGDLL_ALLLANGUAGES

; Language Selection Dialog Settings

  ; Remember the installer language
  !define MUI_LANGDLL_REGISTRY_ROOT "HKCU" 
  !define MUI_LANGDLL_REGISTRY_KEY "Software\Skype Historian" 
  !define MUI_LANGDLL_REGISTRY_VALUENAME "Installer Language"
    
; Pages

    !insertmacro MUI_PAGE_WELCOME
    !insertmacro MUI_PAGE_DIRECTORY
    !insertmacro MUI_PAGE_INSTFILES
    !insertmacro MUI_PAGE_FINISH

    !insertmacro MUI_UNPAGE_WELCOME
    !insertmacro MUI_UNPAGE_CONFIRM
    !insertmacro MUI_UNPAGE_INSTFILES
    !insertmacro MUI_UNPAGE_FINISH

; Languages

    !insertmacro MUI_LANGUAGE "English"
    !insertmacro MUI_LANGUAGE "Russian"

; Installer sections

Section 

    SetOutPath $INSTDIR
    
    File "Skype Historian\bin\Release\Skype Historian.exe"
    File "Skype Historian\bin\Release\ICSharpCode.SharpZipLib.dll"
    File "Skype Historian\bin\Release\Newtonsoft.Json.dll"
    File "Skype Historian\bin\Release\NLog.config"
    File "Skype Historian\bin\Release\NLog.dll"
    File "Skype Historian\bin\Release\Skype4COM.dll"
    File "Skype Historian\bin\Release\Skype4COMWrapper.dll"
    File "Skype Historian\bin\Release\WPFToolkit.dll"
    File "Skype Historian\bin\Release\WPFToolkit.Extended.dll"
    
    File "Externals\Skype4COM.dll.manifest"
    
    SetOutPath $INSTDIR\ru
    
    File "Skype Historian\bin\Release\ru\Skype Historian.resources.dll"
    
    ; Store installation folder
    WriteRegStr HKCU "Software\Skype Historian" "" $INSTDIR
    
    ; Create uninstaller
    WriteUninstaller $INSTDIR\Uninstaller.exe

SectionEnd

; Installer functions

Function .onInit

    !insertmacro MUI_LANGDLL_DISPLAY
    
FunctionEnd

; Uninstaller Section

Section "Uninstall"

    Delete "$INSTDIR\Uninstaller.exe"
    RMDir "$INSTDIR"

    DeleteRegKey HKCU "Software\Skype Historian"

SectionEnd