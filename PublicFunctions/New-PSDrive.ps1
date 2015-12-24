$newPSDriveCommand = Get-Command New-PSDrive

function New-PSDrive {
    [cmdletbinding()]
    param(
        [string]$ComputerName,
        [string]$Name,
        [string]$ServerUrl,
        [string]$PersonalAccessToken
    )

    if (!$ComputerName -and !$Name) {
        throw 'Computer name or name must be specified.'
    }

    if (!$ComputerName -and !$ServerUrl) {
        throw 'Computer name or server URL must be specified.'
    }

    if (!$Name) {
        if ($ComputerName -notmatch '^\d+\.\d+.\d+.\d+$') {
            $Name = $ComputerName.Split('.')[0]
        } else {
            $Name = $ComputerName
        }
    }

    if (!$ServerUrl) {
        $ServerUrl = "http://$($ComputerName):8080/tfs"
    }

    [bool]$useDefaultCredentials = "$PersonalAccessToken" -eq ""
    [bool]$usePersonalAccessToken = "$PersonalAccessToken" -ne ""
    & $newPSDriveCommand -Scope global -Name $Name -PSProvider Vsts -Root $ServerUrl -UseDefaultCredentials:$useDefaultCredentials -UsePersonalAccessToken:$usePersonalAccessToken -PersonalAccessToken $PersonalAccessToken
}