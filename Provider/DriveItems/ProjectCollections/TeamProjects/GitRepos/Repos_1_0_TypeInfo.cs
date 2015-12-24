namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects.GitRepos
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Reflection;

    public sealed class Repos_1_0_TypeInfo : WellKnownNameContainerTypeInfo
    {
        public Repos_1_0_TypeInfo()
        {
            this.AddChildTypeInfo(new Repo_1_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "Repos_1.0";
            }
        }

        public static IEnumerable<PSObject> Create(PSObject psObject, string name)
        {
            // Lookup the project.
            PSObject project = GetProject(psObject);

            // Format the relative URL.
            Repos_1_0_TypeInfo typeInfo = psObject.GetPSVstsTypeInfo() as Repos_1_0_TypeInfo;
            Segment parentSegment = psObject.GetPSVstsParentSegment();
            string relativeUrl = typeInfo.UrlStringFormat(
                "{0}/_apis/git/repositories?api-version=1.0",
                SegmentHelper.FindProjectCollectionName(parentSegment));

            // Format the body JSON content.
            const string BodyJsonFormat = @"{{
    ""name"": ""{0}"",
    ""project"": {{
        ""id"": ""{1}""
    }}
}}";
            string bodyJson = typeInfo.JsonStringFormat(
                BodyJsonFormat,
                name,
                project.Properties["id"].Value);

            // Invoke the HTTP POST request.
            return typeInfo.InvokePostWebRequest(
                provider: psObject.GetPSVstsProvider(),
                relativeUrl: relativeUrl,
                bodyJson: bodyJson);
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.Methods.Add(new PSCodeMethod("Create", this.GetType().GetMethod("Create", BindingFlags.Public | BindingFlags.Static)));
            return psObject;
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            return this.InvokeGetWebRequest(
                segment,
                "{0}/_apis/git/repositories?api-version=1.0&projectId={1}",
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment));
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            return this.InvokeGetWebRequest(
                segment,
                "{0}/_apis/git/repositories/{2}?api-version=1.0&projectId={1}",
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment),
                childSegment.Name);
        }

        private static PSObject GetProject(PSObject psObject)
        {
            Segment parentSegment = psObject.GetPSVstsParentSegment();
            const string Script = @"
[cmdletbinding()]
param(
    [string]$DriveName = $(throw 'Missing DriveName.'),
    [string]$CollectionName = $(throw 'Missing CollectionName.'),
    [string]$ProjectName = $(throw 'Missing ProjectName.')
)

[string]$projectPath = ""$($DriveName):\ProjectCollections_1.0-preview.2\$CollectionName\TeamProjects_1.0\$ProjectName""
Get-Item $projectPath
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
                })
                .Single();
        }
    }
}
