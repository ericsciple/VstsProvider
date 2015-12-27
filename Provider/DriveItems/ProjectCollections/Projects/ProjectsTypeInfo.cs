namespace VstsProvider.DriveItems.ProjectCollections.Projects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;
    using DriveItems = VstsProvider.DriveItems;

    public class ProjectsTypeInfo : HttpClientContainerTypeInfo
    {
        public ProjectsTypeInfo()
            : this(new ProjectTypeInfo())
        {
        }

        public ProjectsTypeInfo(DriveItems::TypeInfo childTypeInfo)
        {
            this.AddChildTypeInfo(childTypeInfo);
        }

        public override string Name
        {
            get
            {
                return "Projects";
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.ProjectCollections.Projects.GetChildDriveItems(Segment)");
            ProjectHttpClient httpClient = this.GetHttpClient(segment) as ProjectHttpClient;
            return this.Wrap(
                segment,
                (int? top, int? skip) =>
                {
                    return httpClient
                        .GetProjects(
                            stateFilter: null,
                            top: top,
                            skip: skip,
                            userState: null)
                        .Result
                        .Select(x => this.ConvertToChildDriveItem(segment, x))
                        .ToArray();
                });
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            segment.GetProvider().WriteDebug("DriveItems.ProjectCollections.Projects.GetChildDriveItems(Segment, Segment)");
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            ProjectHttpClient httpClient = this.GetHttpClient(segment) as ProjectHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    return new[] {
                        this.ConvertToChildDriveItem(
                            segment,
                            httpClient
                            .GetProject(
                                id: childSegment.Name,
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
                    SegmentHelper.FindProjectCollectionName(parentSegment));
        }
    }
}
