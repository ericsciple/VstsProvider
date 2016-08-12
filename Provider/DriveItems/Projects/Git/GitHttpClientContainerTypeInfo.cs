namespace VstsProvider.DriveItems.Projects.Git
{
    using Microsoft.TeamFoundation.SourceControl.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

    public abstract class GitHttpClientContainerTypeInfo : HttpClientContainerTypeInfo
    {
        protected sealed override VssHttpClientBase GetHttpClient(Segment parentSegment)
        {
            DriveInfo driveInfo = parentSegment.GetProvider().PSVstsDriveInfo;
            return driveInfo.GetHttpClient<GitHttpClient>(
                SegmentHelper.GetHttpClientProjectCollectionName(driveInfo.Root, parentSegment));
        }
    }
}
