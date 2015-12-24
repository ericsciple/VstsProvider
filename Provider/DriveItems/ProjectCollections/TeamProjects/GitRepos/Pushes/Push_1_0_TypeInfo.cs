namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects.GitRepos.Pushes
{
    using System.Management.Automation;

    public sealed class Push_1_0_TypeInfo : LeafTypeInfo
    {
        public Push_1_0_TypeInfo()
        {
        }

        public override string Name
        {
            get
            {
                return "Push_1.0";
            }
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.AddPSVstsName(((int)psObject.Properties["pushId"].Value).ToString());
            return psObject;
        }
    }
}
