namespace VstsProvider.DriveItems
{
    using System.Collections.Generic;

    public sealed class RootSegment : Segment
    {
        public RootSegment(Provider provider, Path path, string name, string remainingNames)
            : base(
                provider: provider,
                path: path,
                typeInfo: RootTypeInfo.Instance,
                name: name,
                remainingNames: remainingNames,
                parent: null)
        {
        }
    }
}