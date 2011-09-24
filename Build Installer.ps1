
function Set-Version
{
    Param ([string]$fileName, [string]$version)
    $tempFileName = $fileName + ".tmp"
    Get-Content $fileName |
        %{$_ -replace 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $NewVersion } |
        %{$_ -replace 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $NewFileVersion } > $tempFileName
    # Move-Item $tempFileName $fileName -Force
}

function Get-Version
{
    
}

Write-Output 'Building Skype Historian ...'
'C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild' 'Skype Historian.sln' /t:Rebuild /p:Configuration=Release /p:PlatformToolset=v90
if ($LastExitCode -ne 0)
{
    Write-Error 'Build Failed.'
}
else
{
    Write-Output 'Build Succeeded.'
}
