function Restore-GitRepos {
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
        ForEach-Object { $repos[$_.PSVstsChildName] = $_ }

    # Get the repo directories.
    $repoDirectories = Get-ChildItem -LiteralPath "$($ProjectDirectory)\GitRepos" -ErrorAction Ignore |
        Where-Object { $_ -is [System.IO.DirectoryInfo] }
    foreach ($repoDirectory in $repoDirectories) {
        $repoSegment = $repoDirectory.Name
        $repoName = ConvertFrom-EscapedSegment $repoSegment
        $repo = $repos[$repoDirectory.Name]
        if (!$repo) {
            # Create the repo.
            $repo = New-Object Microsoft.TeamFoundation.SourceControl.WebApi.GitRepository
            $repo.Name = $repoName
            $gitHttpClient = Get-Item -LiteralPath $reposVstsPath
            Write-Verbose "Creating repo: $repoName"
            $null = $gitHttpClient.CreateRepositoryAsync($repo, $projectName).Result
        } else {
            Write-Verbose "Repo exists: $repoName"
        }

        #Restore-Repos -ProjectVstsPath $projectVstsPath -ProjectDirectory $projectDirectory.FullName
    }
}
