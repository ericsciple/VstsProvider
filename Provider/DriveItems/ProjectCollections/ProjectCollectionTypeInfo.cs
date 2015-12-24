namespace VstsProvider.DriveItems.ProjectCollections
{
    using System.Management.Automation;
    //using BuildQueues;
    //using Processes;
    using Projects;

    public sealed class ProjectCollectionTypeInfo : ContainerTypeInfo
    {
        public ProjectCollectionTypeInfo()
        {
            //this.AddChildTypeInfo(new BuildQueues_2_0_TypeInfo());
            //this.AddChildTypeInfo(new GitTeamProjects_1_0_TypeInfo());
            //this.AddChildTypeInfo(new Processes_1_0_TypeInfo());
            this.AddChildTypeInfo(new ProjectsTypeInfo());
        }

        public override string Name
        {
            get
            {
                return "ProjectCollection";
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
