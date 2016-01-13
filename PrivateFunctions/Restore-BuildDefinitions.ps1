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

    # Get the build definitions.
    $definitionsVstsPath = "$ProjectVstsPath\BuildDefinitions"
    $definitions = @{ }
    Write-Verbose "Getting build definitions: $definitionsVstsPath"
    Get-ChildItem -LiteralPath $definitionsVstsPath |
        ForEach-Object { $definitions[$_.Name] = $_ }

    # Get the queues.
    $queues = @{ }
    $httpClient = Get-Item -LiteralPath $definitionsVstsPath
    Write-Verbose "Getting queues"
    $httpClient.GetQueuesAsync().Result |
        ForEach-Object { $queues[$_.Name] = $_ }

    # Get the definition files.
    $definitionsDirectory = "$ProjectDirectory\BuildDefs"
    $serializer = New-Object System.Runtime.Serialization.Json.DataContractJsonSerializer([Microsoft.TeamFoundation.Build.WebApi.BuildDefinition])
    foreach ($file in Get-ChildItem -LiteralPath $definitionsDirectory -Filter *.json -ErrorAction Ignore) {
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

        $repo = $repos[$definition.Repository.Name]
        $queue = $queues[$definition.Queue.Name]
        if (!$queue) {
            Write-Error "Queue not found: $($definition.Queue.Name) ; Project: $projectName"
            continue
        }

        # Fix up the definition.
        $definition.Id = 0
        $definition.ParentDefinition = $null
        if ($repo) {
            $definition.Repository.Id = $repo.Id
            $definition.Repository.Url = $repo.Url
        } elseif ($definition.Repository.Id -eq "$/") {
            $definition.Repository.Url = $httpClient.BaseAddress
        }

        $definition.Queue = $queue

        # Create the definition.
        Write-Verbose "Creating definition: $($definition.Name)"
        $null = $httpClient.CreateDefinitionAsync($definition, $projectName).Result
    }
}
