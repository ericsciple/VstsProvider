function ConvertFrom-EscapedSegment {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name)

    $Name.
        Replace('%5C', '\').
        Replace('%2F', '/').
        Replace('%3A', ':').
        #Replace('???', '*').
        Replace('%3F', '?').
        Replace('%22', '"').
        Replace('%3C', '<').
        Replace('%3E', '>').
        Replace('%7C', '|').
        Replace('%25', '%') # Must be last for unescaping. Consider a non-escaped string: %5C. The escaped
                            # version of the string is: %255C. For unescaping, if the percent replacement
                            # were processed first then the result would be: \. And the result would not
                            # match the original non-escaped string: %5C.
}