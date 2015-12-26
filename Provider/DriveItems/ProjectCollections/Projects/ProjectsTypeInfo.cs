namespace VstsProvider.DriveItems.ProjectCollections.Projects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;
    using Microsoft.TeamFoundation.Core.WebApi;
    using DriveItems = VstsProvider.DriveItems;

    public class ProjectsTypeInfo : WellKnownNameContainerTypeInfo
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

        public static ProjectHttpClient GetHttpClient(PSObject psObject)
        {
            return psObject
                .GetPSVstsProvider()
                .PSVstsDriveInfo
                .GetHttpClient<ProjectHttpClient>(
                    SegmentHelper.FindProjectCollectionName(psObject.GetPSVstsParentSegment()));
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.Methods.Add(new PSCodeMethod("GetHttpClient", this.GetType().GetMethod("GetHttpClient", BindingFlags.Public | BindingFlags.Static)));
            return psObject;
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.ProjectCollections.Projects.GetChildDriveItems(Segment)");
            return this.Wrap((int? top, int? skip) =>
            {
                return segment.GetProvider()
                    .PSVstsDriveInfo
                    .GetHttpClient<ProjectHttpClient>(
                        SegmentHelper.FindProjectCollectionName(segment))
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

            return this.Wrap(() =>
            {
                return new[] {
                    this.ConvertToChildDriveItem(
                        segment,
                        segment
                        .GetProvider()
                        .PSVstsDriveInfo
                        .GetHttpClient<ProjectHttpClient>(
                            SegmentHelper.FindProjectCollectionName(segment))
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
