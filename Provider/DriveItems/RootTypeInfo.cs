namespace VsoProvider.DriveItems
{
    using System.Management.Automation;
    using ProjectCollections;

    public sealed class RootTypeInfo : ContainerTypeInfo
    {
        public static readonly RootTypeInfo Instance = new RootTypeInfo();

        private RootTypeInfo()
        {
            this.AddChildTypeInfo(new ProjectCollections_1_0_preview_2_TypeInfo());
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.AddPSVsoName(this.Name);
            return psObject;
        }

        public sealed override string Name
        {
            get
            {
                return "Root";
            }
        }
    }
}
