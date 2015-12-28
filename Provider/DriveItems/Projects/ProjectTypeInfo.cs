namespace VstsProvider.DriveItems.Projects
{
    using System.Management.Automation;
    using VstsProvider.DriveItems.Projects.Build;
    using VstsProvider.DriveItems.Projects.Git;

    public class ProjectTypeInfo : ContainerTypeInfo
    {
        public ProjectTypeInfo()
        {
            this.AddChildTypeInfo(new BuildDefinitionsTypeInfo());
            this.AddChildTypeInfo(new BuildsTypeInfo());
            this.AddChildTypeInfo(new ReposTypeInfo());
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
