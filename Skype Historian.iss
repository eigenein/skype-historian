[Setup]
AppName=Skype Historian
AppVersion=0.1.41.0
DefaultDirName={localappdata}\Skype Historian
DefaultGroupName=Skype Historian
AllowNoIcons=no
AlwaysUsePersonalGroup=yes
AppComments=Allows you to backup all your Skype chats history
AppContact=eigenein@gmail.com
AppCopyright=Pavel Perestoronin (c) 2011
AppPublisher=Pavel Perestoronin
OutputDir=..\Installers
PrivilegesRequired=lowest
SolidCompression=yes
SetupIconFile=SetupIconFile.ico
UninstallDisplayIcon="{app}\Skype Historian.exe"
UninstallDisplayName=Skype Historian
VersionInfoVersion=0.1.41.0
WizardImageFile=compiler:WizModernImage-IS.bmp
WizardSmallImageFile=compiler:WizModernSmallImage-IS.bmp

[Dirs]
Name: "{app}\ru"

[Files]
Source: ".\Skype Historian\bin\Release\Skype Historian.exe"; DestDir: "{app}"
Source: ".\Skype Historian\bin\Release\ICSharpCode.SharpZipLib.dll"; DestDir: "{app}"
Source: ".\Skype Historian\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"
Source: ".\Skype Historian\bin\Release\NLog.config"; DestDir: "{app}"
Source: ".\Skype Historian\bin\Release\NLog.dll"; DestDir: "{app}"
Source: ".\Skype Historian\bin\Release\Skype4COMWrapper.dll"; DestDir: "{app}"
Source: ".\Skype Historian\bin\Release\WPFToolkit.dll"; DestDir: "{app}"
Source: ".\Skype Historian\bin\Release\WPFToolkit.Extended.dll"; DestDir: "{app}"
Source: ".\Externals\Skype4COM.dll"; DestDir: "{app}"
Source: ".\Externals\Skype4COM.dll.manifest"; DestDir: "{app}"
Source: ".\Skype Historian\bin\Release\ru\Skype Historian.resources.dll"; DestDir: "{app}\ru"

[Icons]
Name: "{group}\Skype Historian"; Filename: "{app}\Skype Historian.exe"; WorkingDir: "{app}"
Name: "{group}\{cm:Uninstall}"; Filename: "{uninstallexe}"
Name: "{userdesktop}\Skype Historian"; Filename: "{app}\Skype Historian.exe"; WorkingDir: "{app}"

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "ru"; MessagesFile: "compiler:Languages\Russian.isl"

[CustomMessages]
en.Uninstall=Uninstall the application
ru.Uninstall=Удалить приложение

[UninstallDelete]
Type: files; Name: "{app}\Skype Historian.log"
Type: files; Name: "{app}\Skype Historian.log.*"
