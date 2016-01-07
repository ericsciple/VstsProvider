function Restore-BuildDefinitions {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$ProjectVstsPath,
        [Parameter(Mandatory = $true)]
        [string]$ProjectDirectory)

    $projectName = ConvertFrom-EscapedSegment ([System.IO.Path]::GetFileName($ProjectVstsPath))

    # Get the repos.
    $repos = @{ }
    $reposVstsPath = "$ProjectVstsPath\GitRepos"
    Write-Verbose "Getting repos: $reposVstsPath"
    Get-ChildItem -LiteralPath $reposVstsPath |
        ForEach-Object { $repos[$_.Name] = $_ }

    # Get the queues.
    $queues = @{ }
    $httpClient = Get-Item -LiteralPath $reposVstsPath
    Write-Verbose "Getting queues"
    $httpClient.GetQueuesAsync().Result |
        ForEach-Object { $queues[$_.Name] = $_ }

    # Get the build definitions.
    $definitionsVstsPath = "$ProjectVstsPath\BuildDefinitions"
    $definitions = @{ }
    Write-Verbose "Getting build definitions: $definitionsVstsPath"
    Get-ChildItem -LiteralPath $definitionsVstsPath |
        ForEach-Object { $definitions[$_.Name] = $_ }

    # Get the definition files.
    $definitionsDirectory = "$ProjectDirectory\BuildDefs"
    $serializer = New-Object System.Runtime.Serialization.Json.DataContractJsonSerializer([Microsoft.TeamFoundation.Build.WebApi.BuildDefinition])
    foreach ($file in Get-ChildItem -LiteralPath $definitionsDirectory -Filter *.json) {
        $stream = $null
        try {
            $stream = New-Object System.IO.FileStream($file.FullName, 'Open')
            $definition = [Microsoft.TeamFoundation.Build.WebApi.BuildDefinition]$serializer.ReadObject($stream)
        } finally {
            if ($stream) { $stream.Dispose() }
        }

        if ($definitions[$definition.Name]) {
            continue
        }

        # Fixup the definition's repo.
        $repo = $repos[$definition.Repository.Name]
        if (!$repo) {
            Write-Error "Repo not found: $($definition.Repository.Name) ; Project: $projectName"
            continue
        }

        $definition.Repository = $repo

        # Fixup the definition's queue.
        $queue = $queues[$definition.Queue.Name]
        if (!$queue) {
            Write-Error "Queue not found: $($definition.Queue.Name) ; Project: $projectName"
            continue
        }

        $definition.Queue = $queue

        # Create the definition.
        Write-Verbose "Creating definition: $($definition.Name)"
        $null = $httpClient.CreateDefinitionAsync($definition, $project).Result
    }
}
