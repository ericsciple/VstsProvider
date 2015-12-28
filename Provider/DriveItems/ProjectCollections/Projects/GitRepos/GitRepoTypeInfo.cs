namespace VstsProvider.DriveItems.ProjectCollections.Projects.GitRepos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    public sealed class GitRepoTypeInfo : LeafTypeInfo
    {
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
