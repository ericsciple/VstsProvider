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
            foreach (var o in segment.GetProvider().PSVstsDriveInfo.GetHttpClient<ProjectCollectionHttpClient>().GetProjectCollections(top: 0, skip: 0, userState: null).Result)
            {
                yield return this.ConvertToDriveItem(segment, o);
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            segment.GetProvider().WriteVerbose("DriveItems.ProjectCollections_1_0_preview_2.GetChildDriveItems(Segment,Segment)");
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            return this.InvokeGetWebRequest(
                segment,
                "_apis/projectcollections/{0}?&api-version=1.0-preview.2",
                childSegment.Name);
        }
    }
}
