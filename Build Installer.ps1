
function Set-Version
{
    Param ([string]$filePath, [string]$version)
    $tempFilePath = $filePath + ".tmp"
    Get-Content $filePath |
        %{[regex]::replace($_, 'AssemblyVersion\("[0-9]+(\.([0-9]+)){1,3}"\)', "AssemblyVersion(""${version}"")")} |
        %{[regex]::replace($_, 'AssemblyFileVersion\("[0-9]+(\.([0-9]+)){1,3}"\)', "AssemblyFileVersion(""${version}"")")} > $tempFilePath
    Move-Item $tempFilePath $filePath -Force
}

function Get-Version
{
    Param([string]$filePath)
    return Get-Content $filePath | 
        %{[regex]::matches($_, 'AssemblyVersion\("(?<version>[0-9]+(\.([0-9]+)){1,3})"\)')} | %{$_.Groups['version'].value}
}

Write-Output 'Updating Version ...'
$currentVersion = Get-Version 'Skype Historian\Properties\AssemblyInfo.cs'
Write-Output "Current Version ${currentVersion}"
$currentVersionObject = New-Object Version $currentVersion
$nextVersionObject = New-Object Version($currentVersionObject.Major, $currentVersionObject.Minor, ($currentVersionObject.Build + 1), $currentVersionObject.Revision)
$nextVersion = $nextVersionObject.ToString()
Write-Output "Updating to ${nextVersion}"
Set-Version 'Skype Historian\Properties\AssemblyInfo.cs' $nextVersion
Write-Output 'Building Skype Historian ...'
C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild 'Skype Historian.sln' /t:Rebuild /p:Configuration=Release /p:PlatformToolset=v90
if ($LastExitCode -ne 0)
{
    Write-Error 'Build Failed.'
}
else
{
    Write-Output 'Building Installer ...'
    & 'C:\Program Files (x86)\Inno Setup 5\iscc' 'Skype Historian.iss'
    if ($LastExitCode -eq 0)
    {
        Move-Item '..\Installers\Setup.exe' "..\Installers\Skype Historian ${nextVersion} Setup.exe" -Force
    }
    else
    {
        Write-Error 'Installer Build Failed'
    }
}
