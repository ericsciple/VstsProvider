namespace VstsProvider.DriveItems.Projects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    //using System.Reflection;
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

    public sealed class ProjectCollectionsTypeInfo : HttpClientContainerTypeInfo
    {
        public ProjectCollectionsTypeInfo()
        {
            this.AddChildTypeInfo(new ProjectCollectionTypeInfo());
        }

        public override string Name
        {
            get
            {
                return "ProjectCollections";
            }
        }

        public override IEnumerable<PSObject> GetItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.Projects.ProjectCollections.GetItems(Segment)");
            ProjectCollectionHttpClient httpClient = this.GetHttpClient(segment) as ProjectCollectionHttpClient;
            return this.Wrap(
                segment,
                (int? top, int? skip) =>
                {
                    return httpClient
                        .GetProjectCollections(
                            top: top,
                            skip: skip,
                            userState: null)
                        .Result
                        .Select(x => this.ConvertToChildDriveItem(segment, x))
                        .ToArray();
                });
        }

        public override IEnumerable<PSObject> GetLiteralItem(Segment segment, Segment childSegment)
        {
            segment.GetProvider().WriteDebug("DriveItems.Projects.ProjectCollections.GetLiteralItem(Segment,Segment)");
            ProjectCollectionHttpClient httpClient = this.GetHttpClient(segment) as ProjectCollectionHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    return new[] {
                        this.ConvertToChildDriveItem(
                            segment,
                            httpClient
                            .GetProjectCollection(
                                id: childSegment.UnescapedName,
                                userState: null)
                            .Result)
                    };
                });
        }

        protected override VssHttpClientBase GetHttpClient(Segment parentSegment)
        {
            return parentSegment
                .GetProvider()
                .PSVstsDriveInfo
                .GetHttpClient<ProjectCollectionHttpClient>();
        }
    }
}
