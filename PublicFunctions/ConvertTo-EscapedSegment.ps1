function ConvertFrom-EscapedSegment {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name)

    $Name.
        Replace('%', '%25'). # Must be first for escaping.
        Replace('\', '%5C').
        Replace('/', '%2F').
        Replace(':', '%3A').
        #Replace('*', '???').
        Replace('?', '%3F').
        Replace('"', '%22').
        Replace('<', '%3C').
        Replace('>', '%3E').
        Replace('|', '%7C')
}