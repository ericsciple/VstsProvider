# TODO: FIX THIS TO CALL THE SEGMENT HELPER.

function ConvertFrom-EscapedSegment {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name)

    $Name.
        Replace('%5C', '\').
        Replace('%2F', '/').
        #Replace('%3A', ':').
        #Replace('???', '*').
        #Replace('%3F', '?').
        #Replace('%22', '"').
        #Replace('%3C', '<').
        #Replace('%3E', '>').
        #Replace('%7C', '|').
        Replace('%25', '%') # Must be unescaped last.
}