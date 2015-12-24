namespace VsoProvider.DriveItems.ProjectCollections.TeamProjects.Builds
{
    using System;
    using System.Management.Automation;

    public sealed class CompletedBuild_2_0_TypeInfo : LeafTypeInfo
    {
        public override string Name
        {
            get
            {
                return "CompletedBuild_2.0";
            }
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            PSPropertyInfo buildNumberPropertyInfo = psObject.Properties["buildNumber"];
            string name;
            if (buildNumberPropertyInfo == null)
            {
                name = ((int)psObject.Properties["id"].Value).ToString();
                parentSegment.GetProvider().WriteWarning(string.Format("Unknown build number. Setting PSVsoName to build ID instead: {0}", name));
            }
            else
            {
                name = buildNumberPropertyInfo.Value as string;
            }

            psObject.AddPSVsoName(name);
            return psObject;
        }
    }
}
