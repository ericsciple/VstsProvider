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
        $repoVstsPath = "$reposVstsPath\$repoSegment"
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

        # Get the refs.
        $refs = @{ }
        $refsVstsPath = "$repoVstsPath\Refs"
        Write-Verbose "Getting refs: $refsVstsPath"
        Get-ChildItem -LiteralPath $refsVstsPath |
            ForEach-Object { $refs[$_.PSVstsChildName] = $_ }

        # Get the ref directories.
        $refDirectories = Get-ChildItem -LiteralPath "$($repoDirectory.FullName)\Refs" -ErrorAction Ignore |
            Where-Object { $_ -is [System.IO.DirectoryInfo] }
        foreach ($refDirectory in $refDirectories) {
            $refSegment = $refDirectory.Name
            $refVstsPath = "$refsVstsPath\$refSegment"
            $refName = ConvertFrom-EscapedSegment $refSegment
            if ($refName -notlike 'heads/*') {
                continue
            }

            # Get the blob items.
            $blobItems = @{ }
            if ($refs[$refSegment]) {
                $itemsVstsPath = "$refVstsPath\Items"
                Write-Verbose "Getting items: $itemsVstsPath"
                Get-ChildItem -LiteralPath $itemsVstsPath |
                    Where-Object { $_.GitObjectType -eq 'Blob' } |
                    ForEach-Object { $blobItems[$_.Path] = $_ }
            }

            # Get the file items.
            $files = Get-ChildItem -LiteralPath "$($refDirectory.FullName)\Items" -Recurse |
                Where-Object { $_ -is [System.IO.FileInfo] }
            foreach ($file in $files) {
                $gitPath = $file.FullName.Substring("$($refDirectory.FullName)\Items".Length).Replace('\', '/')
                if ($blobItems[$gitPath]) {
                    Write-Debug "Exists: $gitPath"
                } else {
                    #Write-Verbose "Not exists: $gitPath"
                }
            }
        }
    }
}
