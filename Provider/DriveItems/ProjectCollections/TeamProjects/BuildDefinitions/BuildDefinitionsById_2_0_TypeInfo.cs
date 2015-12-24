namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects.BuildDefinitions
{
    using System.Collections.Generic;
    using System.Management.Automation;

    public sealed class BuildDefinitionsById_2_0_TypeInfo : WellKnownNameContainerTypeInfo
    {
        public BuildDefinitionsById_2_0_TypeInfo()
        {
            this.AddChildTypeInfo(new BuildDefinitionById_2_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "BuildDefinitionsById_2.0";
            }
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

            return this.InvokeGetWebRequest(
                segment,
                "{0}/{1}/_apis/build/definitions/{2}?api-version=2.0&propertyFilters={3}",
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment),
                childSegment.Name,
                "build");
        }
    }
}
