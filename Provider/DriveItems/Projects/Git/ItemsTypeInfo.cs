namespace VstsProvider.DriveItems.Projects.Git
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.SourceControl.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

    public sealed class ItemsTypeInfo : GitHttpClientContainerTypeInfo
    {
        public ItemsTypeInfo()
        {
            this.AddChildTypeInfo(new ItemTypeInfo());
        }

        public override string Name
        {
            get
            {
                return "Items";
            }
        }

        public override IEnumerable<PSObject> GetItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.Projects.Git.Items.GetItems(Segment)");
            GitHttpClient httpClient = this.GetHttpClient(segment) as GitHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    GitVersionDescriptor versionDescriptor = new GitVersionDescriptor();
                    versionDescriptor.Version = SegmentHelper.GetBranchName(segment);
                    versionDescriptor.VersionType = GitVersionType.Branch;
                    return httpClient
                        .GetItemsAsync(
                            project: SegmentHelper.GetProjectName(segment),
                            repositoryId: SegmentHelper.GetRepoName(segment),
                            recursionLevel: VersionControlRecursionType.Full,
                            versionDescriptor: versionDescriptor)
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
                    GitVersionDescriptor versionDescriptor = new GitVersionDescriptor();
                    versionDescriptor.Version = SegmentHelper.GetBranchName(segment);
                    versionDescriptor.VersionType = GitVersionType.Branch;
                    return new[] {
                        this.ConvertToChildDriveItem(
                            segment,
                            httpClient
                            .GetItemAsync(
                                project: SegmentHelper.GetProjectName(segment),
                                repositoryId: SegmentHelper.GetRepoName(segment),
                                path: Uri.UnescapeDataString(childSegment.UnescapedName),
                                versionDescriptor: versionDescriptor)
                            .Result)
                    };
                });
        }
    }
}
