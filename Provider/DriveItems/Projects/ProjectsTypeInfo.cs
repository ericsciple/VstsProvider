namespace VstsProvider.DriveItems.Projects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    //using System.Reflection;
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;
    //using DriveItems = VstsProvider.DriveItems;

    public class ProjectsTypeInfo : HttpClientContainerTypeInfo
    {
        public ProjectsTypeInfo()
        {
            this.AddChildTypeInfo(new ProjectTypeInfo());
        }

        public override string Name
        {
            get
            {
                return "Projects";
            }
        }

        public override IEnumerable<PSObject> GetItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.Projects.Projects.GetItems(Segment)");
            ProjectHttpClient httpClient = this.GetHttpClient(segment) as ProjectHttpClient;
            return this.Wrap(
                segment,
                (int? top, int? skip) =>
                {
                    return httpClient
                        .GetProjects(
                            stateFilter: Microsoft.TeamFoundation.Common.ProjectState.All,
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
            segment.GetProvider().WriteDebug("DriveItems.Projects.Projects.GetLiteralItem(Segment, Segment)");
            ProjectHttpClient httpClient = this.GetHttpClient(segment) as ProjectHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    // TODO: THIS SHOULD NOT THROW IF NOTHING IS RETURNED. VERIFY.
                    return new[] {
                        this.ConvertToChildDriveItem(
                            segment,
                            httpClient
                            .GetProject(
                                id: childSegment.UnescapedName,
                                includeCapabilities: true,
                                includeHistory: true,
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
                .GetHttpClient<ProjectHttpClient>(
                    SegmentHelper.GetProjectCollectionName(parentSegment));
        }
    }
}
