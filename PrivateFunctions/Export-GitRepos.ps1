function Export-GitRepos {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$ProjectVstsPath,
        [Parameter(Mandatory = $true)]
        [string]$ProjectDirectory)

    # Get the repos.
    $reposVstsPath = "$projectVstsPath\GitRepos"
    $gitHttpClient = Get-Item -LiteralPath $reposVstsPath
    Write-Verbose "Getting Git repos: $reposVstsPath"
    foreach ($repo in Get-ChildItem -LiteralPath $reposVstsPath) {
        $repoSegment = $repo.PSVstsChildName
        $repoVstsPath = "$reposVstsPath\$repoSegment"
        $repoDirectory = "$ProjectDirectory\GitRepos\$repoSegment"
        $null = [System.IO.Directory]::CreateDirectory($repoDirectory)

        # Get the refs.
        $refsVstsPath = "$repoVstsPath\Refs"
        $refsDirectory = "$repoDirectory\Refs"
        Write-Verbose "Getting refs: $refsVstsPath"
        foreach ($ref in Get-ChildItem -LiteralPath $refsVstsPath) {
            if ($ref.Name -notlike 'refs/heads/*') {
                continue
            }

            $refSegment = $ref.PSVstsChildName
            $refVstsPath = "$refsVstsPath\$refSegment"
            $refDirectory = "$refsDirectory\$refSegment"
            $null = [System.IO.Directory]::CreateDirectory($refDirectory)

            # Get the items.
            $itemsVstsPath = "$refVstsPath\Items"
            $itemsDirectory = "$refDirectory\Items"
            Write-Verbose "Getting items: $itemsVstsPath"
            foreach ($item in Get-ChildItem -LiteralPath $itemsVstsPath) {
                if ($item.GitObjectType -ne 'Blob') {
                    continue
                }

                # Download the blob.
                $itemSegment = $item.PSVstsChildName
                $itemVstsPath = "$itemsVstsPath\$itemSegment"
                $itemFile = "$itemsDirectory\$($item.Path.Substring(1).Replace('/', '\'))"
                $itemDirectory = [System.IO.Path]::GetDirectoryName($itemFile)
                $null = [System.IO.Directory]::CreateDirectory($itemDirectory)
                $fileStream = $null
                $downloadStream = $null
                Write-Verbose "Getting item: $itemVstsPath"
                try{
                    $fileStream = New-Object System.IO.FileStream($itemFile, [System.IO.FileMode]::Create)
                    $downloadStream = $gitHttpClient.GetBlobContentAsync(
                        $project.Id,
                        $repo.Id,
                        $item.ObjectId,
                        $true # download
                        ).Result
                    $downloadStream.CopyTo($fileStream)
                    $fileStream.Flush()
                    $fileStream.Close()
                } finally {
                    if ($fileStream) { $fileStream.Dispose() }
                    if ($downloadStream) { $downloadStream.Dispose() }
                }
            }
        }
    }
}
