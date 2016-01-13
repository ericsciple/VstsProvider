namespace VstsProvider.DriveItems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Segment
    {
        private readonly Segment parent;
        private readonly Path path;
        private readonly Provider provider;

        public Segment(Provider provider, Path path, TypeInfo typeInfo, string name, Queue<string> remainingNames, Segment parent)
        {
            this.provider = provider;
            this.path = path;
            this.ItemTypeInfo = typeInfo;
            this.Name = name;
            this.parent = parent;
            if (remainingNames.Count == 0)
            {
                return;
            }

            // Validate the item type info is known for this segment.
            if (this.ItemTypeInfo == null)
            {
                this.path.ThrowInvalid("Unknown path segment type cannot contain a child segment.");
            }

            // Validate this segment is a container.
            if (this.ItemTypeInfo is LeafTypeInfo)
            {
                this.path.ThrowInvalid("Leaf path segments cannot contain a child segment.");
            }

            // Determine the child segment's type info.
            string childName = remainingNames.Dequeue();
            Dictionary<string, TypeInfo> childTypeInfos = (this.ItemTypeInfo as ContainerTypeInfo).ChildTypeInfo;
            TypeInfo childTypeInfo;
            if (childTypeInfos.Count == 1
                && !(childTypeInfos.Values.Single() is HttpClientContainerTypeInfo))
            {
                // Child is always one type (and not an HTTP client).
                childTypeInfo = childTypeInfos.Values.Single();
            }
            else
            {
                TypeInfo[] matchingHttpClientChildTypeInfos =
                    childTypeInfos
                    .Keys
                    .Where(x => x.StartsWith(childName, StringComparison.OrdinalIgnoreCase))
                    .Select(x => childTypeInfos[x])
                    .Where(x => x is HttpClientContainerTypeInfo)
                    .ToArray();
                if (matchingHttpClientChildTypeInfos.Length > 1)
                {
                    // More than one HTTP client partial match found.
                    this.path.ThrowInvalid(string.Format("Ambiguous partial match for segment '{0}'.", childName));
                    throw new Exception("Previous statement should throw.");
                }
                else if (matchingHttpClientChildTypeInfos.Length == 1)
                {
                    // HTTP client found by name or partial name. 
                    childTypeInfo = matchingHttpClientChildTypeInfos.Single();

                    // // Fix the child name, otherwise the partial name can cause strange issues.
                    // // Haven't figured out exactly why, but fixing the child name to match
                    // // works around the issue. For example, if the child name isn't fixed then
                    // // "gi onprem:\proj\defaultcollection" ends up attempting to resolve the
                    // // literal path "onprem:\defaultcollection" instead of "onprem:\proj\defaultcollection".
                    // childName = childTypeInfo.Name;
                }
                else
                {
                    childTypeInfo = null;
                }
            }

            // Create the child segment.
            this.Child = new Segment(
                provider: this.provider,
                path: this.path,
                typeInfo: childTypeInfo,
                name: childName,
                remainingNames: remainingNames,
                parent: this);
        }

        public Segment Child { get; private set; }

        public TypeInfo ItemTypeInfo { get; private set; }

        public string Name { get; private set; }

        public string UnescapedName
        {
            get
            {
                return SegmentHelper.Unescape(this.Name);
            }
        }

        public Segment GetParent()
        {
            return this.parent;
        }

        public Provider GetProvider()
        {
            return this.provider;
        }
    }
}
