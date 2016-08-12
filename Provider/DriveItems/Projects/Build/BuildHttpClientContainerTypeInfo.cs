namespace VstsProvider.DriveItems.Projects.Build
{
    using Microsoft.TeamFoundation.Build.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

    public abstract class BuildHttpClientContainerTypeInfo : HttpClientContainerTypeInfo
    {
        protected sealed override VssHttpClientBase GetHttpClient(Segment parentSegment)
        {
            DriveInfo driveInfo = parentSegment.GetProvider().PSVstsDriveInfo;
            return driveInfo.GetHttpClient<BuildHttpClient>(
                SegmentHelper.GetHttpClientProjectCollectionName(driveInfo.Root, parentSegment));
        }
    }
}
