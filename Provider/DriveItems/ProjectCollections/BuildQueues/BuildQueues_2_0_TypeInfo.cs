namespace VsoProvider.DriveItems.ProjectCollections.BuildQueues
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    public sealed class BuildQueues_2_0_TypeInfo : WellKnownNameContainerTypeInfo
    {
        public BuildQueues_2_0_TypeInfo()
        {
            this.AddChildTypeInfo(new BuildQueue_2_0_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "BuildQueues_2.0";
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            return this.InvokeGetWebRequest(
                segment,
                "{0}/_apis/build/queues?api-version=2.0",
                SegmentHelper.FindProjectCollectionName(segment));
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            // TODO: I THINK THERE MAY BE A BUG HERE. IF AGENT POOLS AND CONTROLLERS CAN HAVE THE SAME NAME, THEN SINGLEORDEFAULT() WILL BE PROBLEMATIC. MAY NEED TO CONVERT THIS CLASS TO ABSTRACT AND HAVE CONCRETE CLASSES AgentPoolBuildQueues_2_0_TypeInfo and BuildControllerBuildQueues_2_0_TypeInfo INSTEAD.
            PSObject childDriveItem =
                this.InvokeGetWebRequest(
                    segment,
                    "{0}/_apis/build/queues?name={1}&api-version=2.0",
                    SegmentHelper.FindProjectCollectionName(segment),
                    childSegment.Name)
                .Where(x => string.Equals(x.GetPSVsoName(), childSegment.Name, StringComparison.OrdinalIgnoreCase))
                .SingleOrDefault();
            return new[] { childDriveItem };
        }
    }
}
