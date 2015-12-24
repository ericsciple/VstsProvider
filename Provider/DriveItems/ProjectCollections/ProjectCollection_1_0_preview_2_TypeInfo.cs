namespace VsoProvider.DriveItems.ProjectCollections
{
    using System.Management.Automation;
    using BuildQueues;
    using Processes;
    using TeamProjects;

    public sealed class ProjectCollection_1_0_preview_2_TypeInfo : ContainerTypeInfo
    {
        public ProjectCollection_1_0_preview_2_TypeInfo()
        {
            this.AddChildTypeInfo(new BuildQueues_2_0_TypeInfo());
            this.AddChildTypeInfo(new GitTeamProjects_1_0_TypeInfo());
            this.AddChildTypeInfo(new Processes_1_0_TypeInfo());
            this.AddChildTypeInfo(new TeamProjects_1_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "ProjectCollection_1.0-preview.2";
            }
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.AddPSVsoName(psObject.Properties["name"].Value as string);
            return psObject;
        }
    }
}
