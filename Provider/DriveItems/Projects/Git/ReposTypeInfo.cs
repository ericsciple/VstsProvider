namespace VstsProvider.DriveItems.Projects.Git
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.SourceControl.WebApi;

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

        public override IEnumerable<PSObject> GetItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.Projects.Git.Repos.GetItems(Segment)");
            GitHttpClient httpClient = this.GetHttpClient(segment) as GitHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    return httpClient
                        .GetRepositoriesAsync(
                            project: SegmentHelper.GetProjectName(segment),
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
                    return new[] {
                        this.ConvertToChildDriveItem(
                            segment,
                            httpClient
                            .GetRepositoryAsync(
                                project: SegmentHelper.GetProjectName(segment),
                                repositoryId: childSegment.UnescapedName)
                            .Result)
                    };
                });
        }
    }
}
