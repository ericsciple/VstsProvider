namespace VstsProvider.DriveItems.Projects.Git
{
    using Microsoft.TeamFoundation.SourceControl.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

    public abstract class GitHttpClientContainerTypeInfo : HttpClientContainerTypeInfo
    {
        protected sealed override VssHttpClientBase GetHttpClient(Segment parentSegment)
        {
            return parentSegment
                .GetProvider()
                .PSVstsDriveInfo
                .GetHttpClient<GitHttpClient>(
                    SegmentHelper.GetProjectCollectionName(parentSegment));
        }
    }
}
