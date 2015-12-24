namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects.BuildDefinitions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Reflection;

    public sealed class BuildDefinition_2_0_TypeInfo : LeafTypeInfo
    {
        public override string Name
        {
            get
            {
                return "BuildDefinition_2.0";
            }
        }

        public static IEnumerable<PSObject> Delete(PSObject psObject)
        {
            // Format the relative URL.
            BuildDefinition_2_0_TypeInfo typeInfo = psObject.GetPSVstsTypeInfo() as BuildDefinition_2_0_TypeInfo;
            Segment parentSegment = psObject.GetPSVstsParentSegment();
            string relativeUrl = typeInfo.UrlStringFormat(
                "{0}/{1}/_apis/build/definitions/{2}?api-version=2.0",
                SegmentHelper.FindProjectCollectionName(parentSegment),
                SegmentHelper.FindTeamProjectName(parentSegment),
                psObject.Properties["id"].Value);

            // Invoke the HTTP DELETE web request.
            yield return typeInfo.InvokeDeleteWebRequest(
                    provider: parentSegment.GetProvider(),
                    relativeUrl: relativeUrl)
                .Single();
        }

        public static IEnumerable<PSObject> ExportToCurrentDirectory(PSObject psObject)
        {
            return ExportToDirectory(psObject, @".");
        }

        public static IEnumerable<PSObject> ExportToDirectory(PSObject psObject, string literalPath)
        {
            string filePath = string.Format(
                @"{0}\{1}.json",
                literalPath.TrimEnd('\\', '/'),
                psObject.Properties["name"].Value as string);
            return ExportToFile(psObject, filePath);
        }

        public static IEnumerable<PSObject> ExportToFile(PSObject psObject, string literalPath)
        {
            Segment parentSegment = psObject.GetPSVstsParentSegment();
            System.IO.File.AppendAllLines(@"c:\temp\temp.txt", new[] { literalPath });
            const string Script = @"
[cmdletbinding()]
param(
    [psobject]$Definition = $(throw 'Missing Definition.'),
    [string]$LiteralPath = $(throw 'Missing LiteralPath.')
)

[string]$directory = Split-Path -Parent $LiteralPath
if ($directory -and
    (!(Test-Path -LiteralPath $directory -PathType Container))) {
    New-Item -Path $directory -ItemType Directory | Out-Null
}

$Definition |
    ConvertTo-Json -Depth 1000 |
    Out-File -LiteralPath $LiteralPath
";
            return parentSegment.GetProvider().InvokeCommand.InvokeScript(
                script: Script,
                useNewScope: true,
                writeToPipeline: PipelineResultTypes.None,
                input: null,
                args: new object[] {
                    GetByIdAndRemovePSProperties(psObject),
                    literalPath,
                });
        }

        public static IEnumerable<PSObject> Queue(PSObject psObject)
        {
            // Format the relative URL.
            BuildDefinition_2_0_TypeInfo typeInfo = psObject.GetPSVstsTypeInfo() as BuildDefinition_2_0_TypeInfo;
            Segment parentSegment = psObject.GetPSVstsParentSegment();
            string relativeUrl = typeInfo.UrlStringFormat(
                "{0}/{1}/_apis/build/builds?api-version=2.0",
                SegmentHelper.FindProjectCollectionName(parentSegment),
                SegmentHelper.FindTeamProjectName(parentSegment));

            // Format the body JSON.
            const string BodyJsonFormat = @"{{
    ""definition"": {{
        ""id"": {0}
    }}
}}";
            string bodyJson = typeInfo.JsonStringFormat(
                BodyJsonFormat,
                psObject.Properties["id"].Value);

            // POST the HTTP web request.
            yield return typeInfo.InvokePostWebRequest(
                    provider: parentSegment.GetProvider(),
                    relativeUrl: relativeUrl,
                    bodyJson: bodyJson)
                .Single();
        }

        public static IEnumerable<PSObject> Rename(PSObject psObject, string newName)
        {
            // Format the relative URL.
            BuildDefinition_2_0_TypeInfo typeInfo = psObject.GetPSVstsTypeInfo() as BuildDefinition_2_0_TypeInfo;
            Segment parentSegment = psObject.GetPSVstsParentSegment();
            string relativeUrl = typeInfo.UrlStringFormat(
                "{0}/{1}/_apis/build/definitions/{2}?api-version=2.0",
                SegmentHelper.FindProjectCollectionName(parentSegment),
                SegmentHelper.FindTeamProjectName(parentSegment),
                psObject.Properties["id"].Value);

            // Get the definition by ID and update the name.
            PSObject definitionById = GetByIdAndRemovePSProperties(psObject);
            definitionById.Properties["name"].Value = newName;

            // Invoke the HTTP PUT web request.
            yield return typeInfo.InvokePutWebRequest(
                    provider: parentSegment.GetProvider(),
                    relativeUrl: relativeUrl,
                    psObject: definitionById)
                .Single();
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.AddPSVstsName(psObject.Properties["name"].Value as string);
            psObject.Methods.Add(new PSCodeMethod("Delete", this.GetType().GetMethod("Delete", BindingFlags.Public | BindingFlags.Static)));
            psObject.Methods.Add(new PSCodeMethod("ExportToFile", this.GetType().GetMethod("ExportToFile", BindingFlags.Public | BindingFlags.Static)));
            psObject.Methods.Add(new PSCodeMethod("ExportToCurrentDirectory", this.GetType().GetMethod("ExportToCurrentDirectory", BindingFlags.Public | BindingFlags.Static)));
            psObject.Methods.Add(new PSCodeMethod("ExportToDirectory", this.GetType().GetMethod("ExportToDirectory", BindingFlags.Public | BindingFlags.Static)));
            psObject.Methods.Add(new PSCodeMethod("Queue", this.GetType().GetMethod("Queue", BindingFlags.Public | BindingFlags.Static)));
            psObject.Methods.Add(new PSCodeMethod("Rename", this.GetType().GetMethod("Rename", BindingFlags.Public | BindingFlags.Static)));
            return psObject;
        }

        private static PSObject GetByIdAndRemovePSProperties(PSObject psObject)
        {
            Segment parentSegment = psObject.GetPSVstsParentSegment();
            const string Script = @"
[cmdletbinding()]
param(
    [string]$DriveName = $(throw 'Missing DriveName.'),
    [string]$CollectionName = $(throw 'Missing CollectionName.'),
    [string]$ProjectName = $(throw 'Missing ProjectName.'),
    [int]$DefinitionId = $(throw 'Missing DefinitionId.')
)

[string]$definitionPath = ""$($DriveName):\ProjectCollections_1.0-preview.2\$CollectionName\TeamProjects_1.0\$ProjectName\BuildDefinitionsById_2.0\$DefinitionId""
Get-Item $definitionPath |
    Select-Object -Property * -ExcludeProperty PS*
";
            return parentSegment.GetProvider().InvokeCommand.InvokeScript(
                script: Script,
                useNewScope: true,
                writeToPipeline: PipelineResultTypes.None,
                input: null,
                args: new object[] {
                    parentSegment.GetProvider().PSVstsDriveInfo.Name,
                    SegmentHelper.FindProjectCollectionName(parentSegment),
                    SegmentHelper.FindTeamProjectName(parentSegment),
                    psObject.Properties["id"].Value,
                })
                .Single();
        }
    }
}
