namespace VstsProvider.DriveItems.Projects.Git
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    public sealed class RepoTypeInfo : ContainerTypeInfo
    {
        public RepoTypeInfo()
        {
            //this.AddChildTypeInfo(new BuildQueues_2_0_TypeInfo());
            //this.AddChildTypeInfo(new GitTeamProjects_1_0_TypeInfo());
            //this.AddChildTypeInfo(new Processes_1_0_TypeInfo());
            this.AddChildTypeInfo(new RefsTypeInfo());
        }

        public override string Name
        {
            get
            {
                return "GitRepo";
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
