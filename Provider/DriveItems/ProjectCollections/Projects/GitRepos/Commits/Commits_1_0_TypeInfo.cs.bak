namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects.GitRepos.Commits
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Management.Automation;
    using System.Reflection;

    public sealed class Commits_1_0_TypeInfo : WellKnownNameContainerTypeInfo
    {
        public Commits_1_0_TypeInfo()
        {
            this.AddChildTypeInfo(new Commit_1_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "Commits_1.0";
            }
        }

        public static IEnumerable<PSObject> GetByItemPath(
            PSObject psObject,
            string path)
        {
            Commits_1_0_TypeInfo typeInfo = psObject.GetPSVstsTypeInfo() as Commits_1_0_TypeInfo;
            Segment parentSegment = psObject.GetPSVstsParentSegment();
            return typeInfo.InvokeGetWebRequest(
                parentSegment,
                "{0}/_apis/git/repositories/{2}/commits?itemPath={3}&api-version=1.0&projectId={1}",
                SegmentHelper.FindProjectCollectionName(parentSegment),
                SegmentHelper.FindTeamProjectName(parentSegment),
                SegmentHelper.FindRepoName(parentSegment),
                path);
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.Methods.Add(new PSCodeMethod("GetByItemPath", this.GetType().GetMethod("GetByItemPath", BindingFlags.Public | BindingFlags.Static)));
            return psObject;
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            return this.InvokeGetWebRequest(
                segment,
                "{0}/_apis/git/repositories/{2}/commits?api-version=1.0&projectId={1}",
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
                "{0}/_apis/git/repositories/{2}/commits/{3}?api-version=2.0-preview&projectId={1}",
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment),
                SegmentHelper.FindRepoName(segment),
                childSegment.Name);
        }
    }
}
