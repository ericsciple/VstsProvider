namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects.GitRepos.Items
{
    using System.Management.Automation;

    public sealed class Item_1_0_TypeInfo : LeafTypeInfo
    {
        public Item_1_0_TypeInfo()
        {
        }

        public override string Name
        {
            get
            {
                return "Item_1.0";
            }
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.AddPSVstsName(psObject.Properties["path"].Value as string);
            return psObject;
        }
    }
}
