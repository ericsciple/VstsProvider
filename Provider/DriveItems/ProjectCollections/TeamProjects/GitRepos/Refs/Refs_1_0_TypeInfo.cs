namespace VsoProvider.DriveItems.ProjectCollections.TeamProjects.GitRepos.Refs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Management.Automation;
    using System.Reflection;

    public sealed class Refs_1_0_TypeInfo : WellKnownNameContainerTypeInfo
    {
        public Refs_1_0_TypeInfo()
        {
            this.AddChildTypeInfo(new Ref_1_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "Refs_1.0";
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            return this.InvokeGetWebRequest(
                segment,
                "{0}/_apis/git/repositories/{2}/refs?api-version=1.0&projectId={1}",
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment),
                SegmentHelper.FindRepoName(segment));
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            if (childSegment.HasWildcard)
            {
                WildcardPattern pattern;
                segment.GetProvider().WriteVerbose("childSegment.HasWildcard");
                pattern = new WildcardPattern(
                    pattern: string.Format("refs/{0}", childSegment.Name.Replace('\\', '/')),
                    options: WildcardOptions.CultureInvariant | WildcardOptions.IgnoreCase);
                return this.GetChildDriveItems(segment)
                    .Where(x => pattern.IsMatch(x.GetPSVsoName()));
            }

            string relativeUrlFormat = string.Format(
                "{{0}}/_apis/git/repositories/{{2}}/refs/{0}?api-version=1.0&projectId={{1}}",
                childSegment.Name.Replace('\\', '/'));
            return this.InvokeGetWebRequest(
                segment,
                relativeUrlFormat,
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment),
                SegmentHelper.FindRepoName(segment));
        }
    }
}
