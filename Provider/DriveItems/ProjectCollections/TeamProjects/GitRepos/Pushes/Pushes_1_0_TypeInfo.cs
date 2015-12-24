namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects.GitRepos.Pushes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Management.Automation;
    using System.Reflection;

    public sealed class Pushes_1_0_TypeInfo : WellKnownNameContainerTypeInfo
    {
        public Pushes_1_0_TypeInfo()
        {
            this.AddChildTypeInfo(new Push_1_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "Pushes_1.0";
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            return this.InvokeGetWebRequest(
                segment,
                "{0}/_apis/git/repositories/{2}/pushes?api-version=1.0&projectId={1}",
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment),
                SegmentHelper.FindRepoName(segment));
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            return this.InvokeGetWebRequest(
                segment,
                "{0}/_apis/git/repositories/{2}/pushes/{3}?api-version=1.0&projectId={1}",
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment),
                SegmentHelper.FindRepoName(segment),
                childSegment.Name);
        }
    }
}
