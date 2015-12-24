namespace VsoProvider.DriveItems.ProjectCollections
{
    using System.Collections.Generic;
    using System.Management.Automation;

    public sealed class ProjectCollections_1_0_preview_2_TypeInfo : WellKnownNameContainerTypeInfo
    {
        public ProjectCollections_1_0_preview_2_TypeInfo()
        {
            this.AddChildTypeInfo(new ProjectCollection_1_0_preview_2_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "ProjectCollections_1.0-preview.2";
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            segment.GetProvider().WriteVerbose("DriveItems.ProjectCollections_1_0_preview_2.GetChildDriveItems(Segment)");
            return this.InvokeGetWebRequest(
                segment,
                "_apis/projectcollections?api-version=1.0-preview.2");
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
