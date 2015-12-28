namespace VstsProvider.DriveItems.Projects.Git
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.SourceControl.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

    public sealed class ReposTypeInfo : GitHttpClientContainerTypeInfo
    {
        public ReposTypeInfo()
        {
            this.AddChildTypeInfo(new RepoTypeInfo());
        }

        public override string Name
        {
            get
            {
                return "GitRepos";
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
                        .GetRepositoriesAsync(
                            project: SegmentHelper.FindProjectName(segment),
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
                    return new[] {
                        this.ConvertToChildDriveItem(
                            segment,
                            httpClient
                            .GetRepositoryAsync(
                                project: SegmentHelper.FindProjectName(segment),
                                repositoryId: childSegment.Name)
                            .Result)
                    };
                });
        }
    }
}
