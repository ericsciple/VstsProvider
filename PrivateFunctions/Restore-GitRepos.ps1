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
        $gitHttpClient = $null
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
            $refName = "refs/$(ConvertFrom-EscapedSegment $refSegment)"
            if ($refName -notlike 'refs/heads/*') {
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

            # Determine the last commit ID.
            $lastCommitId = if ($blobItems.Count) {
                $blobItems[@($blobItems.Keys)[0]].CommitId
            } else {
                '0000000000000000000000000000000000000000'
            }

            # Get the file items.
            $files = Get-ChildItem -LiteralPath "$($refDirectory.FullName)\Items" -Recurse |
                Where-Object { $_ -is [System.IO.FileInfo] }
            foreach ($file in $files) {
                $gitPath = $file.FullName.Substring("$($refDirectory.FullName)\Items".Length).Replace('\', '/')
                if ($blobItems[$gitPath]) {
                    Write-Debug "Exists: $gitPath"
                } else {
                    $binaryExtensions = '.7z', '.doc', '.docx', '.dot', '.fla', '.flv', '.gif', '.gz', '.ico', '.jpeg', '.jpg', '.mov', '.mp3', '.mp4', '.pdf', '.pfx', '.png', '.rtf', '.swf', '.ttf', '.zip'
                    if (([System.IO.Path]::GetExtension($file.FullName)) -in $binaryExtensions) {
                        $content = [System.Convert]::ToBase64String([System.IO.File]::ReadAllBytes($file.FullName), 'None')
                        $contentType = 'base64encoded'
                    } else {
                        $content = [System.IO.File]::ReadAllText($file.FullName).Replace("`r`n", "`n")
                        $contentType = 'rawtext'
                    }

                    $push = New-Object Microsoft.TeamFoundation.SourceControl.WebApi.GitPush -Property @{
                        RefUpdates = [Microsoft.TeamFoundation.SourceControl.WebApi.GitRefUpdate[]]@(
                            New-Object Microsoft.TeamFoundation.SourceControl.WebApi.GitRefUpdate -Property @{
                                Name = $refName
                                OldObjectId = $lastCommitId
                            }
                        )
                        Commits = [Microsoft.TeamFoundation.SourceControl.WebApi.GitCommitRef[]]@(
                            New-Object Microsoft.TeamFoundation.SourceControl.WebApi.GitCommitRef -Property @{
                                Comment = "Restore test data."
                                Changes = [Microsoft.TeamFoundation.SourceControl.WebApi.GitChange[]]@(
                                    New-Object Microsoft.TeamFoundation.SourceControl.WebApi.GitChange -Property @{
                                        ChangeType = 'Add'
                                        Item = New-Object Microsoft.TeamFoundation.SourceControl.WebApi.GitItem -Property @{
                                            Path = $gitPath
                                        }
                                        NewContent = New-Object Microsoft.TeamFoundation.SourceControl.WebApi.ItemContent -Property @{
                                            Content = $content
                                            ContentType = "$contentType"
                                        }
                                    }
                                )
                            }
                        )
                    }

                    if (!$gitHttpClient) {
                        $gitHttpClient = Get-Item -LiteralPath $reposVstsPath
                    }

                    Write-Verbose "Pushing: $($file.FullName)"
                    $createdPush = $gitHttpClient.CreatePushAsync($push, $projectName, $repo.Id).Result
                    $lastCommitId = $createdPush.Commits[0].CommitId
                }
            }

<#
        instance class [mscorlib]System.Threading.Tasks.Task`1<class Microsoft.TeamFoundation.SourceControl.WebApi.GitPush> 
        CreatePushAsync(class Microsoft.TeamFoundation.SourceControl.WebApi.GitPush push,
                        string project,
                        string repositoryId,
                        [opt] object userState,
                        [opt] valuetype [mscorlib]System.Threading.CancellationToken cancellationToken)


    try {
        $defaultCommitId = '0000000000000000000000000000000000000000'
        $pushesApi = Get-Item ".\Pushes_2.0-preview"
        Write-Host "Getting the last commit for branch $Branch in git repo $RepoName in team project $ProjectName."
        try {
            $branchRef = Get-Item ".\Refs_1.0\heads\$BranchName"
            $lastCommitId = $branchRef.objectId
        } catch {
            $lastCommitId = $defaultCommitId
        }

        # Get the files on the server.
        $serverFilePaths = @{ }
        if ($lastCommitId -ne $defaultCommitId) {
            Get-ChildItem ".\Items_1.0" |
                Where-Object { $_.gitobjecttype -eq 'blob' } |
                ForEach-Object { $serverFilePaths[$_.path] = $_.path }
        }

        [string]$codeRoot = Join-Path (Split-Path -Parent -Path $PSScriptRoot) "VsoTestData\GitTeamProjects\$ProjectName\$RepoName\Sources"
        Write-Host "Searching for sources under: $codeRoot"
        Get-ChildItem -Recurse -LiteralPath $codeRoot |
            Where-Object { $_.GetType().Name -eq 'FileInfo' } |
            ForEach-Object {
                [string]$filePath = $_.FullName
                [string]$repoItemPath = $filePath.Substring($codeRoot.Length).Replace('\', '/')
                if (!$serverFilePaths.ContainsKey($repoItemPath)) {
                    [string]$ext = [System.IO.Path]::GetExtension($repoItemPath)
                    if (($ext -eq '.appxmanifest') -or
                        ($ext -eq '.bat') -or
                        ($ext -eq '.cmd') -or
                        ($ext -eq '.config') -or
                        ($ext -eq '.cpp') -or
                        ($ext -eq '.cs') -or
                        ($ext -eq '.csproj') -or
                        ($ext -eq '.filters') -or
                        ($ext -eq '.gitattributes') -or
                        ($ext -eq '.gitignore') -or
                        ($ext -eq '.h') -or
                        ($ext -eq '.hlsl') -or
                        ($ext -eq '.json') -or
                        ($ext -eq '.ps1') -or
                        ($ext -eq '.sln') -or
                        ($ext -eq '.txt') -or
                        ($ext -eq '.vcxproj') -or
                        ($ext -eq '.xaml') -or
                        ($ext -eq '.xml')) {
                        Write-Host "Pushing text file: $repoItemPath"
                        $push = $pushesApi.AddTextFile(
                            "refs/heads/$Branch",
                            $lastCommitId,
                            "Some comment.",
                            $repoItemPath,
                            $filePath,
                            $false) # omitConvertNewLines
                        $lastCommitId = $push.commits[0].commitId
                    } elseif (($ext -eq '.pfx') -or
                        ($ext -eq '.png')) {
                        Write-Host "Pushing binary file: $repoItemPath"
                        $push = $pushesApi.AddBinaryFile(
                            "refs/heads/$Branch",
                            $lastCommitId,
                            "Some comment.",
                            $repoItemPath,
                            $filePath)
                        $lastCommitId = $push.commits[0].commitId
                    } else {
                        throw "Unexpected extension: $ext ; file: $repoItemPath"
                    }
                }
            }
    } finally {
        Pop-Location
    }
#>


        }
    }
}
