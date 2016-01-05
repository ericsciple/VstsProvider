function Export-TeamProjects {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$CollectionVstsPath,
        [Parameter(Mandatory = $true)]
        [string]$CollectionDirectory)

    # Get the projects.
    $projectsVstsPath = "$collectionVstsPath\Projects"
    Write-Verbose "Getting projects: $projectsVstsPath"
    foreach ($project in Get-ChildItem -LiteralPath $projectsVstsPath) {
        $projectSegment = $project.PSVstsChildName
        $projectVstsPath = "$projectsVstsPath\$projectSegment"
        Write-Verbose "Getting project: $projectVstsPath"
        $project = Get-Item -LiteralPath $projectVstsPath
        if (!$?) {
            continue
        }

        $projectDirectory = "$CollectionDirectory\Projs\$projectSegment"
        $null = [System.IO.Directory]::CreateDirectory($projectDirectory)
        $capabilities = New-Object psobject -Property @{
            'processTemplate' = @{
                'templateTypeId' = $project.Capabilities['processTemplate']['templateTypeId']
            }
            'versioncontrol' = @{
                'sourceControlType' = $project.Capabilities['versioncontrol']['sourceControlType']
            }
        }
        $capabilitiesFile = "$projectDirectory\Capabilities.json"
        [System.IO.File]::WriteAllText(
            $capabilitiesFile,
            ($capabilities | ConvertTo-Json -Depth 1000),
            [System.Text.Encoding]::UTF8)
        Export-GitRepos -ProjectVstsPath $projectVstsPath -ProjectDirectory $projectDirectory
    }
}
