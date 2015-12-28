namespace VstsProvider.DriveItems.ProjectCollections.Projects.Builds
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.Build.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

    public sealed class BuildsTypeInfo : BuildHttpClientContainerTypeInfo
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
            BuildHttpClient httpClient = this.GetHttpClient(segment) as BuildHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    return httpClient
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

            BuildHttpClient httpClient = this.GetHttpClient(segment) as BuildHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    return new[] {
                        this.ConvertToChildDriveItem(
                            segment,
                            httpClient
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
