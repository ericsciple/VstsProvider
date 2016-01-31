function Export-TestData {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$VstsDriveName,
        [ValidateNotNullOrEmpty()]
        [Parameter()]
        [string]$LiteralDirectory = "C:\VstsTestData")

    # Get the collections.
    Write-Host "Exporting test data."
    $collectionsVstsPath = "$($VstsDriveName):\ProjectCollections"
    Write-Host "Getting collections: $collectionsVstsPath"
    foreach ($collection in Get-ChildItem -LiteralPath $collectionsVstsPath) {
        # Create the collection directory.
        $collectionSegment = $collection.PSVstsChildName
        $collectionVstsPath = "$collectionsVstsPath\$collectionSegment"
        $collectionDirectory = "$LiteralDirectory\ProjColls\$collectionSegment"
        $null = [System.IO.Directory]::CreateDirectory($collectionDirectory)

        # Export-TeamProjects
        Export-TeamProjects -CollectionVstsPath $collectionVstsPath -CollectionDirectory $collectionDirectory
    }
}