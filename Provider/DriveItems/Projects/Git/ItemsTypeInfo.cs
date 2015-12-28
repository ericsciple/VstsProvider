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

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.Projects.Git.Items.GetChildDriveItems(Segment)");
            GitHttpClient httpClient = this.GetHttpClient(segment) as GitHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    GitVersionDescriptor versionDescriptor = new GitVersionDescriptor();
                    versionDescriptor.Version = SegmentHelper.FindBranchName(segment);
                    versionDescriptor.VersionType = GitVersionType.Branch;
                    return httpClient
                        .GetItemsAsync(
                            project: SegmentHelper.FindProjectName(segment),
                            repositoryId: SegmentHelper.FindRepoName(segment),
                            recursionLevel: VersionControlRecursionType.Full,
                            versionDescriptor: versionDescriptor)
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
                    GitVersionDescriptor versionDescriptor = new GitVersionDescriptor();
                    versionDescriptor.Version = SegmentHelper.FindBranchName(segment);
                    versionDescriptor.VersionType = GitVersionType.Branch;
                    return new[] {
                        this.ConvertToChildDriveItem(
                            segment,
                            httpClient
                            .GetItemAsync(
                                project: SegmentHelper.FindProjectName(segment),
                                repositoryId: SegmentHelper.FindRepoName(segment),
                                path: Uri.UnescapeDataString(childSegment.Name),
                                versionDescriptor: versionDescriptor)
                            .Result)
                    };
                });
        }
    }
}
