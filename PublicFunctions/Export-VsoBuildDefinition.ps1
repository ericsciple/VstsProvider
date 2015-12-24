function Export-VsoBuildDefinition {
    [cmdletbinding()]
    param(
        [string]$LiteralPath = $(throw 'Missing LiteralPath.'),
        [string]$DriveName = $(throw 'Missing DriveName.'),
        [string]$CollectionName = $(throw 'Missing CollectionName.'),
        [string]$ProjectName = $(throw 'Missing ProjectName.'),
        [string]$DefinitionName = $(throw 'Missing DefinitionName.')
        #[Parameter(ParameterSetName = 'ById')]
        #[string]$DefinitionId = $(throw 'Missing DefinitionId.')
    )

    if (!$DefinitionId) {
        $definition = Get-Item "$($DriveName):\ProjectCollections_1.0-preview.2\$CollectionName\TeamProjects_1.0\$ProjectName\BuildDefinitions_2.0\$DefinitionName"
        $DefinitionId = $definition.id
    }

    [string]$directory = Split-Path -Parent $LiteralPath
    if (!(Test-Path -LiteralPath $directory -PathType Container)) {
        New-Item -Path $directory -ItemType Directory | Out-Null
    }

    Get-Item "$($DriveName):\ProjectCollections_1.0-preview.2\$CollectionName\TeamProjects_1.0\$ProjectName\BuildDefinitionsById_2.0\$DefinitionId" |
        Select-Object -Property * -ExcludeProperty PS* |
        ConvertTo-Json -Depth 1000 |
        Out-File -LiteralPath $LiteralPath
}