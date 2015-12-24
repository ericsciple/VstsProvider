namespace VsoProvider.DriveItems.ProjectCollections.TeamProjects.GitRepos.Items
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Management.Automation;
    using System.Reflection;
    using System.Management.Automation.Runspaces;
    using System;
    public sealed class Items_1_0_TypeInfo : WellKnownNameContainerTypeInfo
    {
        public Items_1_0_TypeInfo()
        {
            this.AddChildTypeInfo(new Item_1_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "Items_1.0";
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            return this.InvokeGetWebRequest(
                segment,
                "{0}/_apis/git/repositories/{2}/items?api-version=1.0&scopePath=/&recursionLevel=full&projectId={1}",
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment),
                SegmentHelper.FindRepoName(segment));
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            throw new NotImplementedException();
        }
    }
}
