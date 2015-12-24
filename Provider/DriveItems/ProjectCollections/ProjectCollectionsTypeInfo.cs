namespace VstsProvider.DriveItems.ProjectCollections
{
    using System.Collections.Generic;
    using System.Management.Automation;
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

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.ProjectCollections.GetChildDriveItems(Segment)");
            // TODO: SUPPORT PAGING
            foreach (var o in segment.GetProvider().PSVstsDriveInfo.GetHttpClient<ProjectCollectionHttpClient>().GetProjectCollections(top: 0, skip: 0, userState: null).Result)
            {
                yield return this.ConvertToChildDriveItem(segment, o);
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            segment.GetProvider().WriteDebug("DriveItems.ProjectCollections.GetChildDriveItems(Segment,Segment)");
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            return new[] {
                // TODO: UNRAVEL AGGREGATE EXCEPTION IF ONLY ONE INNER EXCEPTION.
                this.ConvertToChildDriveItem(
                    segment,
                    segment.GetProvider().PSVstsDriveInfo.GetHttpClient<ProjectCollectionHttpClient>().GetProjectCollection(childSegment.Name).Result)
            };
        }
    }
}
