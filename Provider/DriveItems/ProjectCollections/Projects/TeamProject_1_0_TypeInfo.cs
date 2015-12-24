namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects
{
    using System.Management.Automation;
    using Builds;
    using BuildDefinitions;

    public class TeamProject_1_0_TypeInfo : ContainerTypeInfo
    {
        public TeamProject_1_0_TypeInfo()
        {
            this.AddChildTypeInfo(new Builds_2_0_TypeInfo());
            this.AddChildTypeInfo(new BuildDefinitions_2_0_TypeInfo());
            this.AddChildTypeInfo(new BuildDefinitionsById_2_0_TypeInfo());
            this.AddChildTypeInfo(new CompletedBuilds_2_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "TeamProject_1.0";
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
