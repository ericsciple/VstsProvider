namespace VstsProvider.DriveItems.Projects.Git
{
    //using System;
    //using System.Collections.Generic;
    //using System.Linq;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.SourceControl.WebApi;

    public sealed class ItemTypeInfo : LeafTypeInfo
    {
        public override string Name
        {
            get
            {
                return "Item";
            }
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            GitItem item = obj as GitItem;
            PSObject psObject = base.ConvertToDriveItem(parentSegment, item);
            psObject.EscapeAndAddPSVstsChildName(item.Path);
            return psObject;
        }
    }
}
