# VstsProvider
Map a PS drive to Visual Studio Team Services.

## Get/import the module
```
git clone https://github.com/ericsciple/VstsProvider.git
Import-Module .\VstsProvider\VstsProvider.psd1
```

##Map a PS drive to VSTS/TFS
VSTS:
```
New-VstsPSDrive -Name <account> -ServerUrl https://<account>.visualstudio.com -PersonalAccessToken <token>
```

TFS:
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
Or use shorthand:
```
(gi myserver:\projectcollections\defaultcollection\projects).getprojecthistory(0, $null).result | fl *
```