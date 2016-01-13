function Restore-Projects {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$CollectionVstsPath,
        [Parameter(Mandatory = $true)]
        [string]$CollectionDirectory)

    # Get the projects.
    $projects = @{ }
    $projectsVstsPath = "$CollectionVstsPath\Projects"
    Write-Verbose "Getting projects: $projectsVstsPath"
    Get-ChildItem -LiteralPath $projectsVstsPath |
        ForEach-Object { $projects[$_.PSVstsChildName] = $_ }

    # Get the project directories.
    $projectDirectories = Get-ChildItem -LiteralPath "$($CollectionDirectory)\Projs" |
        Where-Object { $_ -is [System.IO.DirectoryInfo] }
    foreach ($projectDirectory in $projectDirectories) {
        $projectSegment = $projectDirectory.Name
        $projectName = ConvertFrom-EscapedSegment $projectSegment
        $project = $projects[$projectDirectory.Name]
        if (!$project) {
            # Load the capabilities file.
            $capabilitiesPath = "$($projectDirectory.FullName)\Capabilities.json"
            $capabilities = ConvertFrom-Json -InputObject (Get-Content -LiteralPath $capabilitiesPath -ErrorAction Stop | Out-String) -ErrorAction Stop

            # Create the project.
            $project = New-Object Microsoft.TeamFoundation.Core.WebApi.TeamProject
            $project.Name = $projectName
            $project.Description = $projectName
            $project.Capabilities = @{ }
            $capabilities |
                Get-Member -MemberType NoteProperty |
                ForEach-Object {
                    $key = $_.Name
                    $capabilities.$key |
                        Get-Member -MemberType NoteProperty |
                        ForEach-Object {
                            $subkey = $_.Name
                            $value = $capabilities.$key.$subkey
                            $project.Capabilities[$key] = @{ }
                            $project.Capabilities[$key][$subkey] = $value
                        }
                }
            $projectHttpClient = Get-Item -LiteralPath $projectsVstsPath
            #$projectHttpClient.DefaultRequestHeaders.Add('Cookie', 'Tfs-EnablePCW=1')
            Write-Verbose "Creating project: $projectName"
            $null = $projectHttpClient.QueueCreateProject($project).Result
            #$projectHttpClient.lastresponsecontext
            #$host.EnterNestedPrompt()
            #throw 'failed :('
        } else {
            Write-Verbose "Project exists: $projectName"
        }

        # Wait for the project to be well formed.
        if ($project.State -ne 'wellFormed') {
            Write-Verbose "Waiting for project '$projectName' to be well-formed."
            while ($project.State -ne 'wellFormed') {
                $project = Get-Item "$CollectionVstsPath\Projects\$projectSegment"
                Write-Verbose "Project '$ProjectName' state: $($project.State)"
                Start-Sleep -Seconds 1
            }
        }

        $projectVstsPath = "$projectsVstsPath\$projectSegment"
        Restore-GitRepos -ProjectVstsPath $projectVstsPath -ProjectDirectory $projectDirectory.FullName
        Restore-BuildDefinitions -ProjectVstsPath $projectVstsPath -ProjectDirectory $projectDirectory.FullName
    }
}
