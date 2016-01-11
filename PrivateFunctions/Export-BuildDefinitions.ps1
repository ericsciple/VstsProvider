function Export-BuildDefinitions {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$ProjectVstsPath,
        [Parameter(Mandatory = $true)]
        [string]$ProjectDirectory)

    $projectName = ConvertFrom-EscapedSegment ([System.IO.Path]::GetFileName($ProjectVstsPath))

    # Get the build definitions.
    $definitionsVstsPath = "$ProjectVstsPath\BuildDefinitions"
    $definitions = @{ }
    Write-Verbose "Getting build definitions: $definitionsVstsPath"
    Get-ChildItem -LiteralPath $definitionsVstsPath |
        ForEach-Object { $definitions[$_.Name] = $_ }

    # Export the definition files.
    $definitionsDirectory = "$ProjectDirectory\BuildDefs"
    $null = [System.IO.Directory]::CreateDirectory($definitionsDirectory)
    $serializer = New-Object System.Runtime.Serialization.Json.DataContractJsonSerializer([Microsoft.TeamFoundation.Build.WebApi.BuildDefinition])
    $httpClient = Get-Item $definitionsVstsPath
    foreach ($definitionName in $definitions.Keys) {
        # Get the full definition.
        $definition = $definitions[$definitionName]
        $definitionSegment = $definition.PSVstsChildName
        $definitionVstsPath = "$definitionsVstsPath\$definitionSegment"
        Write-Verbose "Getting full definition by ID for: $definitionVstsPath"
        if (!($definition = $httpClient.GetDefinitionAsync($projectName, $definition.Id).Result)) {
            continue
        }

        $definitionFile = "$definitionsDirectory\$definitionSegment.json"
        $fileStream = $null
        try{
            $fileStream = New-Object System.IO.FileStream($definitionFile, [System.IO.FileMode]::Create)
            $serializer.WriteObject($fileStream, $definition)
            $fileStream.Flush()
            $fileStream.Close()
        } finally {
            if ($fileStream) { $fileStream.Dispose() }
        }
    }
}
