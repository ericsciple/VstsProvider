namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects.Builds
{
    using System.Collections.Generic;
    using System.Management.Automation;

    public sealed class Builds_2_0_TypeInfo : WellKnownNameContainerTypeInfo
    {
        public Builds_2_0_TypeInfo()
        {
            this.AddChildTypeInfo(new Build_2_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "Builds_2.0";
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            const int Top = 25;
            segment.GetProvider().WriteWarning(string.Format("Getting top {0:N0} only.", Top));
            return this.InvokeGetWebRequest(
                segment,
                "{0}/{1}/_apis/build/builds?$top={2}&api-version=2.0",
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
                "{0}/{1}/_apis/build/builds?buildNumber={2}&api-version=2.0",
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment),
                childSegment.Name);
        }
    }
}
