namespace VstsProvider.DriveItems.Projects.Build
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.Build.WebApi;

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
            Build build = obj as Build;
            PSObject psObject = base.ConvertToDriveItem(parentSegment, build);
            string name;
            if (string.IsNullOrEmpty(build.BuildNumber))
            {
                name = Guid.NewGuid().ToString();
                parentSegment.GetProvider().WriteWarning(string.Format("Unknown build number. Setting PSVstsName: {0}", name));
            }
            else
            {
                name = build.BuildNumber;
            }

            psObject.AddPSVstsName(name);
            return psObject;
        }
    }
}
