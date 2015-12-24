namespace VstsProvider.DriveItems.ProjectCollections.Projects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.Core.WebApi;

    public class ProjectsTypeInfo : WellKnownNameContainerTypeInfo
    {
        public ProjectsTypeInfo()
            : this(new ProjectTypeInfo())
        {
        }

        public ProjectsTypeInfo(TypeInfo childTypeInfo)
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
            // TODO: SUPPORT PAGING
            return this.Wrap(() =>
            {
                return segment.GetProvider()
                    .PSVstsDriveInfo
                    .GetHttpClient<ProjectHttpClient>()
                    .GetProjects(
                        stateFilter: null,
                        top: null,
                        skip: null,
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

            return this.Wrap(() =>
            {
                return new[] {
                    this.ConvertToChildDriveItem(
                        segment,
                        segment
                        .GetProvider()
                        .PSVstsDriveInfo.GetHttpClient<ProjectHttpClient>()
                        .GetProject(
                            id: childSegment.Name,
                            includeCapabilities: true,
                            includeHistory: true,
                            userState: null)
                        .Result)
                };
            });
        }
    }
}
