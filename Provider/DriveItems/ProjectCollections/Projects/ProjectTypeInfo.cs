namespace VstsProvider.DriveItems.ProjectCollections.Projects
{
    using System.Management.Automation;
    using VstsProvider.DriveItems.ProjectCollections.Projects.BuildDefinitions;
    using VstsProvider.DriveItems.ProjectCollections.Projects.Builds;
    using VstsProvider.DriveItems.ProjectCollections.Projects.GitRepos;

    public class ProjectTypeInfo : ContainerTypeInfo
    {
        public ProjectTypeInfo()
        {
            this.AddChildTypeInfo(new BuildDefinitionsTypeInfo());
            this.AddChildTypeInfo(new BuildsTypeInfo());
            this.AddChildTypeInfo(new GitReposTypeInfo());
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
