namespace VsoProvider.DriveItems.ProjectCollections.TeamProjects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    public class TeamProjects_1_0_TypeInfo : WellKnownNameContainerTypeInfo
    {
        public TeamProjects_1_0_TypeInfo()
            : this(new TeamProject_1_0_TypeInfo())
        {
        }

        public TeamProjects_1_0_TypeInfo(TypeInfo childTypeInfo)
        {
            this.AddChildTypeInfo(childTypeInfo);
        }

        public override string Name
        {
            get
            {
                return "TeamProjects_1.0";
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            // TODO: WRAP AND HANDLE $TOP/$SKIP
            return this.InvokeGetWebRequest(
                segment,
                "{0}/_apis/projects?api-version=1.0&stateFilter=All",
                SegmentHelper.FindProjectCollectionName(segment));
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            return this.InvokeGetWebRequest(
                segment,
                "{0}/_apis/projects/{1}?api-version=1.0&stateFilter=All&includeCapabilities=true",
                SegmentHelper.FindProjectCollectionName(segment),
                childSegment.Name);
        }

        public override IEnumerable<PSObject> NewChildDriveItem(Segment segment, Segment childSegment, object dynamicParameters)
        {
            // Format the relative URL.
            string relativeUrl = this.UrlStringFormat(
                "{0}/_apis/projects?api-version=1.0",
                SegmentHelper.FindProjectCollectionName(segment));

            // Format the body JSON.
            TeamProject_1_0_NewItemParameters parameters = dynamicParameters as TeamProject_1_0_NewItemParameters;
            const string BodyJsonFormat = @"{{
    ""name"": ""{0}"",
    ""description"": ""{1}"",
    ""capabilities"": {{
        ""versioncontrol"": {{
            ""sourceControlType"": ""{2}""
        }},
        ""processTemplate"": {{
            ""templateTypeId"": ""{3}""
        }}
    }}
}}";
            string bodyJson = this.JsonStringFormat(
                BodyJsonFormat,
                childSegment.Name,
                parameters.Description,
                parameters.SourceControlType,
                parameters.ProcessTemplateId);

            // POST the HTTP web request.
            return this.InvokePostWebRequest(
                provider: segment.GetProvider(),
                relativeUrl: relativeUrl,
                bodyJson: bodyJson);
        }

        public override object NewChildDriveItemDynamicParameters()
        {
            return new TeamProject_1_0_NewItemParameters();
        }

        protected PSObject GetTeamProjectById(Segment segment, string id)
        {
            return this.InvokeGetWebRequest(
                    segment,
                    "{0}/_apis/projects/{1}?api-version=1.0&stateFilter=All&includeCapabilities=true",
                    SegmentHelper.FindProjectCollectionName(segment),
                    id)
                .Single();
        }

        protected string GetSourceControlType(PSObject teamProjectWithCapabilities)
        {
            PSObject capabilities = teamProjectWithCapabilities.Properties["capabilities"].Value as PSObject;
            PSObject versionControl = capabilities.Properties["versionControl"].Value as PSObject;
            return versionControl.Properties["sourceControlType"].Value as string;
        }
    }
}
