namespace VsoProvider.DriveItems.ProjectCollections.Processes
{
    using System.Management.Automation;

    public sealed class Process_1_0_TypeInfo : LeafTypeInfo
    {
        public override string Name
        {
            get
            {
                return "Process_1.0";
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
