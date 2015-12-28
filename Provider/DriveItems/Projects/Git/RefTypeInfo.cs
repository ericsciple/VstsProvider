namespace VstsProvider.DriveItems.Projects.Git
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.SourceControl.WebApi;

    public sealed class RefTypeInfo : LeafTypeInfo
    {
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
                // This should never happen. Just a sanity check to make sure the API
                // understood.
                throw new Exception(string.Format("Unexpected ref name format: {0}", r.Name));
            }

            PSObject psObject = base.ConvertToDriveItem(parentSegment, r);
            psObject.AddPSVstsName(Uri.EscapeDataString(r.Name.Substring("refs/".Length)));
            return psObject;
        }
    }
}
