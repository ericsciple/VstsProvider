namespace VsoProvider.DriveItems.ProjectCollections.TeamProjects.GitRepos.Refs
{
    using System;
    using System.Management.Automation;

    public sealed class Ref_1_0_TypeInfo : LeafTypeInfo
    {
        public Ref_1_0_TypeInfo()
        {
        }

        public override string Name
        {
            get
            {
                return "Ref_1.0";
            }
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            string name = psObject.Properties["name"].Value as string;
            if (name.StartsWith("refs/", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(startIndex: "refs/".Length);
            }
            else
            {
                throw new Exception(string.Format("Unexpected ref name: {0}", name));
            }

            psObject.AddPSVsoName(name);
            return psObject;
        }
    }
}
