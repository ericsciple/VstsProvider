namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects.BuildDefinitions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Reflection;

    public sealed class BuildDefinitionById_2_0_TypeInfo : LeafTypeInfo
    {
        public override string Name
        {
            get
            {
                return "BuildDefinitionById_2.0";
            }
        }

        ////// TODO: THIS ISN'T SAFE TO DO UNLESS THE OBJECT WAS RETRIEVED BY ID... I.E. GET-ITEM.
        ////public static IEnumerable<PSObject> Update(PSObject psObject)
        ////{
        ////    // Format the relative URL.
        ////    BuildDefinitionById_2_0_TypeInfo typeInfo = psObject.GetPSVstsTypeInfo() as BuildDefinitionById_2_0_TypeInfo;
        ////    Segment parentSegment = psObject.GetPSVstsParentSegment();
        ////    string relativeUrl = typeInfo.UrlStringFormat(
        ////        "{0}/{1}/_apis/build/definitions/{2}?api-version=2.0",
        ////        SegmentHelper.FindProjectCollectionName(parentSegment),
        ////        SegmentHelper.FindTeamProjectName(parentSegment),
        ////        psObject.Properties["id"].Value);

        ////    // Get the definition by ID and update the name.
        ////    PSObject definitionWithoutPSProperties = RemovePSProperties(psObject);

        ////    // Invoke the HTTP PUT web request.
        ////    return typeInfo.InvokePutWebRequest(
        ////            provider: parentSegment.GetProvider(),
        ////            relativeUrl: relativeUrl,
        ////            psObject: definitionWithoutPSProperties);
        ////}

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.AddPSVstsName(((int)psObject.Properties["id"].Value).ToString());
            ////psObject.Methods.Add(new PSCodeMethod("Update", this.GetType().GetMethod("Update", BindingFlags.Public | BindingFlags.Static)));
            return psObject;
        }

        private static PSObject RemovePSProperties(PSObject psObject)
        {
            Segment parentSegment = psObject.GetPSVstsParentSegment();
            const string Script = @"
[cmdletbinding()]
param(
    [psobject]$Definition = $(throw 'Missing Definition.')
)

$Definition |
    Select-Object -Property * -ExcludeProperty PS*
";
            return parentSegment.GetProvider().InvokeCommand.InvokeScript(
                script: Script,
                useNewScope: true,
                writeToPipeline: PipelineResultTypes.None,
                input: null,
                args: new object[] { psObject })
                .Single();
        }
    }
}
