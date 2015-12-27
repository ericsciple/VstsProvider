namespace VstsProvider.DriveItems.ProjectCollections.Projects.BuildDefinitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    public sealed class BuildDefinitionTypeInfo : LeafTypeInfo
    {
        public override string Name
        {
            get
            {
                return "BuildDefinition";
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
