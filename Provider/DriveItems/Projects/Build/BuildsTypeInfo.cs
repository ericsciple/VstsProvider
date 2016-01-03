namespace VstsProvider.DriveItems.Projects.Build
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.Build.WebApi;

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

        public override IEnumerable<PSObject> GetItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.Projects.Build.Builds.GetItems(Segment)");
            BuildHttpClient httpClient = this.GetHttpClient(segment) as BuildHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    return httpClient
                        .GetBuildsAsync(
                            project: SegmentHelper.GetProjectName(segment),
                            top: 25)
                        .Result
                        .Select(x => this.ConvertToChildDriveItem(segment, x))
                        .ToArray();
                });
        }

        public override IEnumerable<PSObject> GetLiteralItem(Segment segment, Segment childSegment)
        {
            segment.GetProvider().WriteDebug("DriveItems.Projects.Build.Builds.GetLiteralItem(Segment, Segment)");
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
                                project: SegmentHelper.GetProjectName(segment),
                                buildNumber: childSegment.UnescapedName)
                            .Result
                            .Single())
                    };
                });
        }
    }
}
