namespace VsoProvider.DriveItems.ProjectCollections.TeamProjects
{
    using VsoProvider.DriveItems.ProjectCollections.TeamProjects.GitRepos;

    public sealed class GitTeamProject_1_0_TypeInfo : TeamProject_1_0_TypeInfo
    {
        public GitTeamProject_1_0_TypeInfo()
        {
            this.AddChildTypeInfo(new Repos_1_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "GitTeamProject_1.0";
            }
        }
    }
}
