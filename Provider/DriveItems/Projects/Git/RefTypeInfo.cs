namespace VstsProvider.DriveItems.Projects.Git
{
    using System;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.SourceControl.WebApi;

    public sealed class RefTypeInfo : ContainerTypeInfo
    {
        public RefTypeInfo()
        {
            this.AddChildTypeInfo(new ItemsTypeInfo());
        }

        public override string Name
        {
            get
            {
                return "Ref";
            }
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            GitRef r = obj as GitRef;
            if (!r.Name.StartsWith("refs/"))
            {
                // This should never happen. Just a sanity check.
                throw new Exception(string.Format("Unexpected ref name format: {0}. Expected format: refs/[...]", r.Name));
            }

            PSObject psObject = base.ConvertToDriveItem(parentSegment, r);
            psObject.EscapeAndAddPSVstsChildName(r.Name.Substring("refs/".Length));
            return psObject;
        }
    }
}
