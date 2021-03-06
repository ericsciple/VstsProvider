@{
    RootModule = 'VstsProvider.psm1'
    ModuleVersion = '0.1'
    GUID = '7cd0d39c-48a8-4bf3-b09b-84644ed45984'
    Author = 'Eric Sciple'
    CompanyName = ''
    Copyright = '(c) 2015 Eric Sciple. All rights reserved.'
    Description = 'VSTS Provider'
    PowerShellVersion = '3.0'
    DotNetFrameworkVersion = '4.5'
    ClrVersion = '4.0'
    TypesToProcess = @( 'VstsProvider.Types.ps1xml' )
    FormatsToProcess = @( 'VstsProvider.Format.ps1xml' )
    FunctionsToExport = '*'
    CmdletsToExport = ''
    VariablesToExport = ''
    AliasesToExport = ''
    PrivateData = @{
        PSData = @{
            # Tags = @()
            # LicenseUri = ''
            ProjectUri = 'https://github.com/ericsciple/VstsProvider'
            # IconUri = ''
            # ReleaseNotes = ''
        }
    }
    HelpInfoURI = 'https://github.com/ericsciple/VstsProvider'
    DefaultCommandPrefix = 'Vsts'
}
