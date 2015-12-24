namespace VsoProvider.DriveItems.ProjectCollections.TeamProjects.GitRepos.Commits
{
    using System.Management.Automation;

    public sealed class Commit_1_0_TypeInfo : LeafTypeInfo
    {
        public Commit_1_0_TypeInfo()
        {
        }

        public override string Name
        {
            get
            {
                return "Commit_1.0";
            }
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.AddPSVsoName(psObject.Properties["commitId"].Value as string);
            return psObject;
        }
    }
}
