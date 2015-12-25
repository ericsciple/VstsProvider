namespace VstsProvider.DriveItems.ProjectCollections.Projects.Builds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    public sealed class BuildTypeInfo : LeafTypeInfo
    {
        public override string Name
        {
            get
            {
                return "Build";
            }
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            PSPropertyInfo buildNumberPropertyInfo = psObject.Properties["buildNumber"];
            string name;
            if (buildNumberPropertyInfo == null)
            {
                name = Guid.NewGuid().ToString();
                parentSegment.GetProvider().WriteWarning(string.Format("Unknown build number. Setting PSVstsName: {0}", name));
            }
            else
            {
                name = psObject.Properties["buildNumber"].Value as string;
            }

            psObject.AddPSVstsName(name);
            return psObject;
        }
    }
}
