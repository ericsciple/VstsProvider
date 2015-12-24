namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects
{
    using System.Management.Automation;

    public sealed class GitTeamProject_1_0_NewItemParameters : TeamProject_1_0_NewItemParameters
    {
        [Parameter()]
        [ValidateSet("Git")]
        public override string SourceControlType
        {
            get
            {
                return "Git";
            }

            set
            {
                //// Intentionally empty.
            }
        }
    }
}
