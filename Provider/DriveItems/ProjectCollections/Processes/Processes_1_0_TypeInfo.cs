namespace VsoProvider.DriveItems.ProjectCollections.Processes
{
    using System.Collections.Generic;
    using System.Management.Automation;

    public sealed class Processes_1_0_TypeInfo : WellKnownNameContainerTypeInfo
    {
        public Processes_1_0_TypeInfo()
        {
            this.AddChildTypeInfo(new Process_1_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "Processes_1.0";
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            return this.InvokeGetWebRequest(
                segment,
                "{0}/_apis/process/processes?api-version=1.0",
                SegmentHelper.FindProjectCollectionName(segment));
        }
    }
}
