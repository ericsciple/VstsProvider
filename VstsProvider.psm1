[CmdletBinding()]
param()

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
& $nugetPath restore "$PSScriptRoot\Provider\packages.config" '-OutputDirectory' "$PSScriptRoot\Provider\packages" '-verbosity' det 2>&1 |
    ForEach-Object {
        if ($_ -is [System.Management.Automation.ErrorRecord]) {
            Write-Error -ErrorRecord $_
        } else {
            Write-Verbose $_
        }
    }
Write-Progress -Activity 'Restoring NuGet packages.' -Completed

# Compile the provider module.
Write-Progress -Activity 'Compiling provider.' -PercentComplete 0
Write-Verbose 'Compiling provider.'
[string[]]$sourceFiles =
    Get-ChildItem -Recurse -Path $PSScriptRoot\Provider\*.cs |
    Select-Object -ExpandProperty FullName
$compilerParameters = New-Object -TypeName System.CodeDom.Compiler.CompilerParameters
$compilerParameters.CompilerOptions = '/unsafe'
$compilerParameters.ReferencedAssemblies.Add('System.dll')
$compilerParameters.ReferencedAssemblies.Add('System.Core.dll')
$compilerParameters.ReferencedAssemblies.Add("$PSScriptRoot\Provider\packages\Microsoft.PowerShell.3.ReferenceAssemblies.1.0.0\lib\net4\System.Management.Automation.dll")
foreach ($dll in Get-ChildItem -LiteralPath @( "$PSScriptRoot\Provider\packages\Newtonsoft.Json.6.0.5\lib\net45", "$PSScriptRoot\Provider\packages\Microsoft.AspNet.WebApi.Client.5.2.2\lib\net45", "$PSScriptRoot\Provider\packages\Microsoft.VisualStudio.Services.Client.14.89.0\lib\net45", "$PSScriptRoot\Provider\packages\Microsoft.TeamFoundationServer.Client.14.89.0\lib\net45" ) -Filter "*.dll") {
    $compilerParameters.ReferencedAssemblies.Add($dll.FullName)
    Add-Type -LiteralPath $dll.FullName
}
# $compilerParameters.ReferencedAssemblies.Add("$PSScriptRoot\Provider\packages\Microsoft.TeamFoundationServer.Client.14.89.0\lib\net45\Microsoft.TeamFoundation.Core.WebApi.dll")
# $compilerParameters.ReferencedAssemblies.Add("$PSScriptRoot\Provider\packages\Microsoft.VisualStudio.Services.Client.14.89.0\lib\net45\Microsoft.TeamFoundation.Common.dll")
# $compilerParameters.ReferencedAssemblies.Add("$PSScriptRoot\Provider\packages\Microsoft.VisualStudio.Services.Client.14.89.0\lib\net45\Microsoft.VisualStudio.Services.Common.dll")
# $compilerParameters.ReferencedAssemblies.Add("$PSScriptRoot\Provider\packages\Microsoft.VisualStudio.Services.Client.14.89.0\lib\net45\Microsoft.VisualStudio.Services.WebApi.dll")
Add-Type -LiteralPath $sourceFiles -CompilerParameters $compilerParameters
Write-Progress -Activity 'Compiling provider.' -Completed

# Import the provider module.
Import-Module -Assembly ([VstsProvider.Provider].Assembly)

# Export public functions.
. "$PSScriptRoot\PublicFunctions\Get-BuildDefinition"
. "$PSScriptRoot\PublicFunctions\Export-BuildDefinition"
. "$PSScriptRoot\PublicFunctions\New-PSDrive"
Export-ModuleMember -Function Get-BuildDefinition
Export-ModuleMember -Function Export-BuildDefinition
Export-ModuleMember -Function New-PSDrive
