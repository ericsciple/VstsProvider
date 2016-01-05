function Restore-TestData {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$VstsDriveName,
        [ValidateNotNullOrEmpty()]
        [Parameter()]
        [string]$LiteralDirectory = "C:\VstsTestData")


    # Get the collections.
    Write-Verbose "Restoring test data."
    $collectionDirectories = Get-ChildItem -LiteralPath "$LiteralDirectory\ProjColls" |
        Where-Object { $_ -is [System.IO.DirectoryInfo] }
    foreach ($collectionDirectory in $collectionDirectories) {
        $collectionSegment = $collectionDirectory.Name
        $collectionVstsPath = "$($VstsDriveName):\ProjectCollections\$collectionSegment"
        # TODO: CREATE THE COLLECTION IF IT DOES NOT EXIST.
        Restore-Projects -CollectionVstsPath $collectionVstsPath -CollectionDirectory $collectionDirectory.FullName
    }
}