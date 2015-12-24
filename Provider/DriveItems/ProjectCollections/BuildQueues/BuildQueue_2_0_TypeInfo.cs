namespace VsoProvider.DriveItems.ProjectCollections.BuildQueues
{
    using System.Management.Automation;

    public sealed class BuildQueue_2_0_TypeInfo : LeafTypeInfo
    {
        public override string Name
        {
            get
            {
                return "BuildQueue_2.0";
            }
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.AddPSVsoName(psObject.Properties["name"].Value as string);
            return psObject;
        }
    }
}
