namespace VstsProvider.DriveItems.ProjectCollections.Projects
{
    using System.Management.Automation;

    public class ProjectTypeInfo : ContainerTypeInfo
    {
        public ProjectTypeInfo()
        {
        }

        public override string Name
        {
            get
            {
                return "Project";
            }
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.AddPSVstsName(psObject.Properties["name"].Value as string);
            return psObject;
        }
    }
}
