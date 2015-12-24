function Get-BuildDefinition {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$DriveName)

    Get-ChildItem "$($DriveName):\ProjectCollections_1.0-preview.2" |
        ForEach-Object {
            $collectionName = $_.Name
            Get-ChildItem "$($DriveName):\ProjectCollections_1.0-preview.2\$collectionName\TeamProjects_1.0" |
                ForEach-Object {
                    $teamProjectName = $_.Name
                    Get-ChildItem "$($DriveName):\ProjectCollections_1.0-preview.2\$collectionName\TeamProjects_1.0\$teamProjectName\BuildDefinitions_2.0"
                }
        }
}