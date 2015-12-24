namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects
{
    using System.Management.Automation;

    public class TeamProject_1_0_NewItemParameters
    {
        [Parameter()]
        public string Description { get; set; }

        [Parameter()]
        public string ProcessTemplateId { get; set; }

        [Parameter()]
        [ValidateSet("Git", "Tfvc")]
        public virtual string SourceControlType { get; set; }
    }
}
