# VstsProvider
Map a PS drive to a Visual Studio Team Services account or Team Foundation Server.

Browse the server from the command line as a directory hierarchy.

Access the REST SDK from the command line.

## Get/import the module
```
git clone https://github.com/ericsciple/VstsProvider.git
Import-Module .\VstsProvider\VstsProvider.psd1
```

## Map a PS drive to VSTS/TFS
VSTS account:
```
New-VstsPSDrive -Name <account> -ServerUrl https://<account>.visualstudio.com -PersonalAccessToken <token>
```

TFS server:
```
New-VstsPSDrive -Name <server> -ServerUrl http://<server>:8080/tfs
```

## Browse/navigate the server (tab completion supported)
```
dir MyServer:\ProjectCollections\DefaultCollection\Projects
gi MyServer:\ProjectCollections\DefaultCollection\Projects\MyProject | fl *
cd MyServer:\ProjectCollections\DefaultCollection
```

## Currently implemented drive items
```
ProjectCollections
ProjectCollections\<collection>\Projects
ProjectCollections\<collection>\Projects\<project>\Builds
ProjectCollections\<collection>\Projects\<project>\BuildDefinitions
ProjectCollections\<collection>\Projects\<project>\GitRepos
ProjectCollections\<collection>\Projects\<project>\GitRepos\<repo>\Refs
ProjectCollections\<collection>\Projects\<project>\GitRepos\<repo>\Refs\<branchspec>\Items
```

## Need more? Use the Rest SDK
Each well-known-name drive item *is the HTTP client*. Get the item, explore it's members, and invoke it's methods:
```
$projectHttpClient = Get-Item MyServer:\ProjectCollections\DefaultCollection\Projects
$projectHttpClient | Get-Member | Format-Table
$projectHttpClient.GetProjectHistory(0, $null).Result | Format-List *
```

## Commands too long?
No need to type the full name for HTTP client drive items. Just type enough of the name to distinguish it from its siblings.
```
gi myserver:\proj\defaultcollection\p\myProject
```

## Export/Restore test data
Currently can be used to export/restore: projects, git repos, git sources (tip of each branch only - not a full clone), and build definitions.

Expect "path too long" issues if your sources contain a somewhat deep structure. Need to spend more time in this area.
```
Export-TestData -VstsDriveName myserver -LiteralDirectory C:\VstsTestData
Restore-TestData -VstsDriveName otherserver -LiteralDirectory C:\VstsTestData
```