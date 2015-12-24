namespace VsoProvider.DriveItems.ProjectCollections.TeamProjects.BuildDefinitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;

    public sealed class BuildDefinitions_2_0_TypeInfo : WellKnownNameContainerTypeInfo
    {
        public BuildDefinitions_2_0_TypeInfo()
        {
            this.AddChildTypeInfo(new BuildDefinition_2_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "BuildDefinitions_2.0";
            }
        }

        public static PSObject Create(PSObject psObject, string definitionJson)
        {
            // Format the relative URL.
            BuildDefinitions_2_0_TypeInfo typeInfo = psObject.GetPSVsoTypeInfo() as BuildDefinitions_2_0_TypeInfo;
            Segment parentSegment = psObject.GetPSVsoParentSegment();
            string relativeUrl = typeInfo.UrlStringFormat(
                "{0}/{1}/_apis/build/definitions?api-version=2.0",
                SegmentHelper.FindProjectCollectionName(parentSegment),
                SegmentHelper.FindTeamProjectName(parentSegment));

            ////psObject.GetPSVsoProvider().WriteWarning(relativeUrl);
            ////psObject.GetPSVsoProvider().WriteWarning(definitionJson);

            // POST the HTTP web request.
            return typeInfo.InvokePostWebRequest(
                    provider: parentSegment.GetProvider(),
                    relativeUrl: relativeUrl,
                    bodyJson: definitionJson)
                .SingleOrDefault();
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
                "{0}/{1}/_apis/build/definitions?api-version=2.0",
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment));
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            PSObject childDriveItem =
                this.InvokeGetWebRequest(
                    segment,
                    "{0}/{1}/_apis/build/definitions?name={2}&api-version=2.0",
                    SegmentHelper.FindProjectCollectionName(segment),
                    SegmentHelper.FindTeamProjectName(segment),
                    childSegment.Name)
                .Where(x => string.Equals(x.GetPSVsoName(), childSegment.Name, StringComparison.OrdinalIgnoreCase))
                .SingleOrDefault();
            return new[] { childDriveItem };
        }
    }
}
