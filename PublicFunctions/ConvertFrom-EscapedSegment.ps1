# TODO: Move this to be a compiled cmdlet so the SegmentHelper class can be marked "internal".
function ConvertFrom-EscapedSegment {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name)

    [VstsProvider.DriveItems.SegmentHelper]::Unescape($Name)
}