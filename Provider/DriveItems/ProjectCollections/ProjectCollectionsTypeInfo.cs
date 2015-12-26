namespace VstsProvider.DriveItems.ProjectCollections
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;
    using Microsoft.TeamFoundation.Core.WebApi;

    public sealed class ProjectCollectionsTypeInfo : WellKnownNameContainerTypeInfo
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

        public static ProjectCollectionHttpClient GetHttpClient(PSObject psObject)
        {
            return psObject
                .GetPSVstsProvider()
                .PSVstsDriveInfo
                .GetHttpClient<ProjectCollectionHttpClient>();
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.Methods.Add(new PSCodeMethod("GetHttpClient", this.GetType().GetMethod("GetHttpClient", BindingFlags.Public | BindingFlags.Static)));
            return psObject;
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.ProjectCollections.GetChildDriveItems(Segment)");
            return this.Wrap((int? top, int? skip) =>
            {
                return segment.GetProvider()
                    .PSVstsDriveInfo
                    .GetHttpClient<ProjectCollectionHttpClient>()
                    .GetProjectCollections(
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
            segment.GetProvider().WriteDebug("DriveItems.ProjectCollections.GetChildDriveItems(Segment,Segment)");
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            return this.Wrap(() =>
            {
                return new[] {
                    // TODO: UNRAVEL AGGREGATE EXCEPTION IF ONLY ONE INNER EXCEPTION.
                    this.ConvertToChildDriveItem(
                        segment,
                        segment
                        .GetProvider()
                        .PSVstsDriveInfo
                        .GetHttpClient<ProjectCollectionHttpClient>()
                        .GetProjectCollection(
                            id: childSegment.Name,
                            userState: null)
                        .Result)
                };
            });
        }
    }
}
