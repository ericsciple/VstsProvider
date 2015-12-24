namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects.GitRepos.Pushes
{
    using System.Management.Automation;

    public sealed class Push_2_0_preview_TypeInfo : LeafTypeInfo
    {
        public Push_2_0_preview_TypeInfo()
        {
        }

        public override string Name
        {
            get
            {
                return "Push_2.0-preview";
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
