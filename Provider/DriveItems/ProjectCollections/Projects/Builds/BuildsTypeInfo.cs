namespace VstsProvider.DriveItems.ProjectCollections.Projects.Builds
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.Build.WebApi;

    public sealed class BuildsTypeInfo : WellKnownNameContainerTypeInfo
    {
        public BuildsTypeInfo()
        {
            this.AddChildTypeInfo(new BuildTypeInfo());
        }

        public override string Name
        {
            get
            {
                return "Builds";
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.ProjectCollections.Projects.Builds.GetChildDriveItems(Segment)");
            return this.Wrap(() =>
            {
                return segment.GetProvider()
                    .PSVstsDriveInfo
                    .GetHttpClient<BuildHttpClient>(
                        SegmentHelper.FindProjectCollectionName(segment))
                    .GetBuildsAsync(
                        project: SegmentHelper.FindProjectName(segment),
                        top: 25)
                    .Result
                    .Select(x => this.ConvertToChildDriveItem(segment, x))
                    .ToArray();
            });
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            segment.GetProvider().WriteDebug("DriveItems.ProjectCollections.Projects.Builds.GetChildDriveItems(Segment, Segment)");
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            return this.Wrap(() =>
            {
                return new[] {
                    this.ConvertToChildDriveItem(
                        segment,
                        segment
                        .GetProvider()
                        .PSVstsDriveInfo
                        .GetHttpClient<BuildHttpClient>(
                            SegmentHelper.FindProjectCollectionName(segment))
                        .GetBuildsAsync(
                            project: SegmentHelper.FindProjectName(segment),
                            buildNumber: childSegment.Name)
                        .Result
                        .Single())
                };
            });
        }
    }
}
