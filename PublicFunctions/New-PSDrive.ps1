$newPSDriveCommand = Get-Command New-PSDrive

function New-PSDrive {
    [cmdletbinding(DefaultParameterSetName = 'ComputerName')]
    param(
        [Parameter(Mandatory = $true, ParameterSetName = 'ComputerName')]
        [string]$ComputerName,
        [ValidateNotNullOrEmpty()]
        [Parameter()]
        [string]$Name,
        [ValidatePattern('^https?://')]
        [Parameter(Mandatory = $true, ParameterSetName = 'ServerUrl')]
        [string]$ServerUrl,
        [Parameter(ParameterSetName = 'ServerUrl')]
        [string]$PersonalAccessToken
    )

    if (!$Name) {
        if ($ComputerName) {
            $host = $ComputerName
        } else {
            $host = (New-Object System.Uri($ServerUrl, [System.UriKind]::Absolute)).Host
        }

        if ($host -notmatch '^\d+\.\d+.\d+.\d+$') {
            $Name = $host.Split('.')[0]
        } else {
            $Name = $host
        }
    }

    if (!$ServerUrl) {
        $ServerUrl = "http://$($ComputerName):8080/tfs"
    }

    # Validate URL format and normalize slashes.
    $scheme = (New-Object System.Uri($ServerUrl, [System.UriKind]::Absolute)).Scheme
    $prefix = "$($scheme)://"
    $ServerUrl = [string]::Concat(
        $prefix,
        $ServerUrl.Substring($prefix.Length).Replace('\', '/').Trim('/'))

    [bool]$useDefaultCredentials = "$PersonalAccessToken" -eq ""
    [bool]$usePersonalAccessToken = "$PersonalAccessToken" -ne ""
    & $newPSDriveCommand -Scope global -Name $Name -PSProvider Vsts -Root $ServerUrl -UseDefaultCredentials:$useDefaultCredentials -UsePersonalAccessToken:$usePersonalAccessToken -PersonalAccessToken $PersonalAccessToken
}