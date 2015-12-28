# VstsProvider
Map a PS drive to Visual Studio Team Services.

## Get/import the module
```
git clone https://github.com/ericsciple/VstsProvider.git
Import-Module .\VstsProvider\VstsProvider.psd1
```

##Map a PS drive to a hosted account
```
New-VstsPSDrive -Name <account> -ServerUrl https://<account>.visualstudio.com -PersonalAccessToken <token>
```

##Map a PS drive to an on-premises server
```
New-VstsPSDrive -Name <server> -ServerUrl http://<server>:8080/tfs
```

## Browse/navigate the server (tab completion supported)
```
dir MyServer:\ProjectCollections\DefaultCollection\Projects
gi MyServer:\ProjectCollections\DefaultCollection\Projects\MyProject | fl *
cd MyServer:\ProjectCollections\DefaultCollection
```

## Use the Rest SDK
```
$projectHttpClient = Get-Item MyServer:\ProjectCollections\DefaultCollection\Projects
$projectHttpClient | Get-Member | Format-Table
$projectHttpClient | gm | ft
```
## Currently supported drive items
```
ProjectCollections
ProjectCollections\<collection>\Projects
ProjectCollections\<collection>\Projects\<project>\Builds
ProjectCollections\<collection>\Projects\<project>\BuildDefinitions
ProjectCollections\<collection>\Projects\<project>\GitRepos
ProjectCollections\<collection>\Projects\<project>\GitRepos\<repo>\Refs
ProjectCollections\<collection>\Projects\<project>\GitRepos\<repo>\Refs\<branchspec>\Items
```