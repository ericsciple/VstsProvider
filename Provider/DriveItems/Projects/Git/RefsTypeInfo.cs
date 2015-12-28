namespace VstsProvider.DriveItems.Projects.Git
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.SourceControl.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

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

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.Projects.Git.Repos.GetChildDriveItems(Segment)");
            GitHttpClient httpClient = this.GetHttpClient(segment) as GitHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    return httpClient
                        .GetRefsAsync(
                            project: SegmentHelper.FindProjectName(segment),
                            repositoryId: SegmentHelper.FindRepoName(segment),
                            includeLinks: true)
                        .Result
                        .Select(x => this.ConvertToChildDriveItem(segment, x))
                        .ToArray();
                });
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            segment.GetProvider().WriteDebug("DriveItems.Projects.Git.Repos.GetChildDriveItems(Segment, Segment)");
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            GitHttpClient httpClient = this.GetHttpClient(segment) as GitHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    string filter = Uri.UnescapeDataString(childSegment.Name);
                    string fullName = string.Format("refs/{0}", filter);
                    segment.GetProvider().WriteWarning(filter);
                    return new[] {
                        this.ConvertToChildDriveItem(
                            segment,
                            httpClient
                            .GetRefsAsync(
                                project: SegmentHelper.FindProjectName(segment),
                                repositoryId: SegmentHelper.FindRepoName(segment),
                                filter: filter,
                                includeLinks: true)
                            .Result
                            .Single(x => string.Equals(x.Name, fullName, StringComparison.OrdinalIgnoreCase)))
                    };
                });
        }
    }
}
