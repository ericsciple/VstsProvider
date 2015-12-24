namespace VsoProvider.DriveItems.ProjectCollections.TeamProjects.Builds
{
    using System.Collections.Generic;
    using System.Management.Automation;

    public sealed class CompletedBuilds_2_0_TypeInfo : WellKnownNameContainerTypeInfo
    {
        public CompletedBuilds_2_0_TypeInfo()
        {
            this.AddChildTypeInfo(new CompletedBuild_2_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "CompletedBuilds_2.0";
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            const int Top = 25;
            return this.InvokeGetWebRequest(
                segment,
                "{0}/{1}/_apis/build/builds?$top={2}&statusFilter=completed&api-version=2.0",
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment),
                Top);
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            return this.InvokeGetWebRequest(
                segment,
                "{0}/{1}/_apis/build/builds?buildNumber={2}&statusFilter=completed&api-version=2.0",
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment),
                childSegment.Name);
        }
    }
}
