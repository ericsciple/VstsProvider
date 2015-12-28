namespace VstsProvider.DriveItems.ProjectCollections.Projects
{
    using Microsoft.TeamFoundation.Build.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

    public abstract class BuildHttpClientContainerTypeInfo : HttpClientContainerTypeInfo
    {
        protected sealed override VssHttpClientBase GetHttpClient(Segment parentSegment)
        {
            return parentSegment
                .GetProvider()
                .PSVstsDriveInfo
                .GetHttpClient<BuildHttpClient>(
                    SegmentHelper.FindProjectCollectionName(parentSegment));
        }
    }
}
