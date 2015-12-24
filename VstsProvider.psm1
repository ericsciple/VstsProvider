[CmdletBinding()]
Param()

# Download NuGet.exe.
$nugetPath = "$PSScriptRoot\Provider\packages\nuget.exe_3.3.0\nuget.exe"
if (!(Test-Path -LiteralPath $nugetPath -PathType Leaf)) {
    $null = [System.IO.Directory]::CreateDirectory([System.IO.Path]::GetDirectoryName($nugetPath))
    Write-Progress -Activity 'Downloading NuGet.exe.' -PercentComplete 0
    Write-Verbose "Downloading NuGet.exe."
    (New-Object System.Net.WebClient).DownloadFile('https://dist.nuget.org/win-x86-commandline/v3.3.0/nuget.exe', $nugetPath)
    Write-Progress -Activity 'Downloading NuGet.exe.' -Completed
}

# Restore NuGet packages.
Write-Progress -Activity 'Restoring NuGet packages.' -PercentComplete 0
Write-Verbose "Restoring NuGet packages."
& $nugetPath restore "$PSScriptRoot\Provider\packages.config" '-OutputDirectory' "$PSScriptRoot\Provider\packages" '-Verbosity' detailed 2>&1 |
    ForEach-Object {
        if ($_ -is [System.Management.Automation.ErrorRecord]) {
            Write-Error -ErrorRecord $_
        } else {
            Write-Verbose $_
        }
    }
sleep -Seconds 5
Write-Progress -Activity 'Restoring NuGet packages.' -Completed

<#
# Compile the provider module.
[string[]]$sourceFiles =
    Get-ChildItem -Recurse -Path $PSScriptRoot\Provider\*.cs |
    Select-Object -ExpandProperty FullName
$compilerParameters = New-Object -TypeName System.CodeDom.Compiler.CompilerParameters
$compilerParameters.CompilerOptions = '/unsafe'
$compilerParameters.ReferencedAssemblies.Add('System.dll')
$compilerParameters.ReferencedAssemblies.Add('System.Core.dll')
$compilerParameters.ReferencedAssemblies.Add("$PSScriptRoot\Provider\packages\System.Management.Automation_PowerShell_3.0.6.3.9600.17400\lib\net40\System.Management.Automation.dll")
Add-Type -LiteralPath $sourceFiles -CompilerParameters $compilerParameters

# Import the provider module.
Import-Module -Assembly ([VsoProvider.Provider].Assembly)

# Export public functions.
. "$PSScriptRoot\PublicFunctions\Get-VsoBuildDefinition"
. "$PSScriptRoot\PublicFunctions\Export-VsoBuildDefinition"
. "$PSScriptRoot\PublicFunctions\Mount-VsoDrive"
Export-ModuleMember -Function Get-VsoBuildDefinition
Export-ModuleMember -Function Export-VsoBuildDefinition
Export-ModuleMember -Function Mount-VsoDrive

# TODO: REMOVE THIS: Map a drive to OnPrem.
Mount-VsoDrive -ComputerName $env:ComputerName -Name OnPrem
#>