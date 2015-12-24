namespace VstsProvider.DriveItems.ProjectCollections.TeamProjects
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;

    public sealed class GitTeamProjects_1_0_TypeInfo : TeamProjects_1_0_TypeInfo
    {
        public GitTeamProjects_1_0_TypeInfo()
            : base(new GitTeamProject_1_0_TypeInfo())
        {
        }

        public override string Name
        {
            get
            {
                return "GitTeamProjects_1.0";
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            foreach (PSObject teamProjectWithoutCapabilities in base.GetChildDriveItems(segment))
            {
                PSObject teamProjectWithCapabilities = this.GetTeamProjectById(
                    segment: segment,
                    id: teamProjectWithoutCapabilities.Properties["id"].Value as string);
                string sourceControlType = this.GetSourceControlType(teamProjectWithCapabilities);
                if (string.Equals(sourceControlType, "Git", StringComparison.OrdinalIgnoreCase))
                {
                    yield return teamProjectWithCapabilities;
                }
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            if (childSegment.HasWildcard)
            {
                foreach (PSObject teamProjectWithoutCapabilities in base.GetChildDriveItems(segment, childSegment))
                {
                    PSObject teamProjectWithCapabilities = this.GetTeamProjectById(
                        segment: segment,
                        id: teamProjectWithoutCapabilities.Properties["id"].Value as string);
                    string sourceControlType = this.GetSourceControlType(teamProjectWithCapabilities);
                    if (string.Equals(sourceControlType, "Git", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return teamProjectWithCapabilities;
                    }
                }
            }
            else
            {
                foreach (PSObject teamProjectWithCapabilities in base.GetChildDriveItems(segment, childSegment))
                {
                    string sourceControlType = this.GetSourceControlType(teamProjectWithCapabilities);
                    if (string.Equals(sourceControlType, "Git", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return teamProjectWithCapabilities;
                    }
                }
            }
        }

        public override object NewChildDriveItemDynamicParameters()
        {
            return new GitTeamProject_1_0_NewItemParameters();
        }
    }
}
