namespace VstsProvider.DriveItems.Projects.Git
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.SourceControl.WebApi;

    public sealed class RefsTypeInfo : GitHttpClientContainerTypeInfo
    {
        public RefsTypeInfo()
        {
            this.AddChildTypeInfo(new RefTypeInfo());
        }

        public override string Name
        {
            get
            {
                return "Refs";
            }
        }

        public override IEnumerable<PSObject> GetItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.Projects.Git.Repos.GetItems(Segment)");
            GitHttpClient httpClient = this.GetHttpClient(segment) as GitHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    return httpClient
                        .GetRefsAsync(
                            project: SegmentHelper.GetProjectName(segment),
                            repositoryId: SegmentHelper.GetRepoName(segment),
                            includeLinks: true)
                        .Result
                        .Select(x => this.ConvertToChildDriveItem(segment, x))
                        .ToArray();
                });
        }

        public override IEnumerable<PSObject> GetLiteralItem(Segment segment, Segment childSegment)
        {
            segment.GetProvider().WriteDebug("DriveItems.Projects.Git.Repos.GetLiteralItem(Segment, Segment)");
            GitHttpClient httpClient = this.GetHttpClient(segment) as GitHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    string filter = childSegment.UnescapedName;
                    string fullName = string.Format("refs/{0}", filter);
                    return new[] {
                        this.ConvertToChildDriveItem(
                            segment,
                            httpClient
                            .GetRefsAsync(
                                project: SegmentHelper.GetProjectName(segment),
                                repositoryId: SegmentHelper.GetRepoName(segment),
                                filter: filter,
                                includeLinks: true)
                            .Result
                            .SingleOrDefault(x => string.Equals(x.Name, fullName, StringComparison.OrdinalIgnoreCase)))
                    };
                });
        }
    }
}
