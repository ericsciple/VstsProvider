namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects.GitRepos
{
    using Commits;
    using Items;
    using Pushes;
    using Refs;
    using System.Management.Automation;

    public sealed class Repo_1_0_TypeInfo : ContainerTypeInfo
    {
        public Repo_1_0_TypeInfo()
        {
            this.AddChildTypeInfo(new Commits_1_0_TypeInfo());
            this.AddChildTypeInfo(new Items_1_0_TypeInfo());
            this.AddChildTypeInfo(new Pushes_1_0_TypeInfo());
            this.AddChildTypeInfo(new Pushes_2_0_preview_TypeInfo());
            this.AddChildTypeInfo(new Refs_1_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "Repo_1.0";
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
